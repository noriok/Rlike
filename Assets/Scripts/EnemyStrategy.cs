using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public static class EnemyStrategy {
    // to に近づく
    private static Loc[] Approach(Loc fm, Loc to) {
        Dir dir = fm.Toward(to);

        var locs = new[] {
            fm.Forward(dir),
            fm.Forward(dir.Clockwise()),
            fm.Forward(dir.Anticlockwise()),
            fm.Forward(dir.Clockwise().Clockwise()),
            fm.Forward(dir.Anticlockwise().Anticlockwise()),
        };

        Array.Sort(locs, (a, b) => {
            var x = a.SquareDistance(to);
            var y = b.SquareDistance(to);
            return x.CompareTo(y);
        });
        return locs;
    }

    // 前進する場合の移動先の候補を返す。進めない場合は後退する。
    private static Loc[] Advance(Loc fm, Dir dir) {
        var locs = new[] {
            fm.Forward(dir),
            fm.Forward(dir.Clockwise()),
            fm.Forward(dir.Anticlockwise()),
            fm.Forward(dir.Clockwise().Clockwise()),
            fm.Forward(dir.Anticlockwise().Anticlockwise()),
            fm.Backward(dir),
        };
        return locs;
    }

    private static bool CanDirectAttack(Loc fm, Loc to, Floor floor) {
        if (fm.IsNeighbor(to)) {
            Dir dir = fm.Toward(to);
            return floor.CanAdvance(fm, dir);
        }
        return false;
    }

    // private static Loc Move(Enemy enemy, Loc[] candidates, Floor floor, bool[,] locs) {
    //     foreach (var loc in candidates) {
    //         Dir dir = enemy.Loc.Toward(loc);
    //         if (!locs[loc.Row, loc.Col] && floor.CanAdvance(enemy.Loc, dir)) {
    //             return loc;
    //         }
    //     }
    //     return false;
    // }

    public static List<Act> Detect(List<Enemy> enemies, Player player, Loc playerNextLoc, Floor floor) {
        // プレイヤーが見えない場合の行動
        if (player.IsInvisible()) {
            return Detect2(enemies, player, playerNextLoc, floor);

        }

        // 敵をプレイヤーに近い距離順にソートする
        // TODO:プレイヤーから逃げる行動をとる場合は、プレイヤーから遠い順に行動を決める
        enemies.Sort((a, b) => {
            var x = a.Loc.SquareDistance(playerNextLoc);
            var y = b.Loc.SquareDistance(playerNextLoc);
            return x.CompareTo(y);
        });

        var q = new List<Act>();

        var locs = new bool[floor.Rows, floor.Cols]; // キャラクターの位置
        var used = new bool[enemies.Count];
        for (int i = 0; i < enemies.Count; i++) {
            if (enemies[i].ActCount <= 0) { // 行動済み
                used[i] = true; // 行動済み
            }
            else if (enemies[i].IsBehavioralIncapacitation()) {
                used[i] = true;
            }
            Loc loc = enemies[i].Loc;
            locs[loc.Row, loc.Col] = true;
        }
        locs[playerNextLoc.Row, playerNextLoc.Col] = true;

        // 移動するキャラ
        bool updated = true;
        while (updated) {
            updated = false;
            for (int i = 0; i < enemies.Count; i++) {
                if (used[i]) continue;

                var enemy = enemies[i];
                Assert.IsTrue(locs[enemy.Loc.Row, enemy.Loc.Col]);

                if (CanDirectAttack(enemy.Loc, playerNextLoc, floor)) continue;

                // 移動先を決める
                if (floor.InSight(enemy.Loc, playerNextLoc)) { // プレイヤーが視界内

                    // 敵が部屋にいて、プレイヤーが部屋の入り口にいるならターゲットとして追尾
                    if (floor.IsRoom(enemy.Loc) && floor.IsEntrance(playerNextLoc)) {
                        enemy.LockOn(playerNextLoc);
                    }
                    else {
                        enemy.CancelTarget();
                    }

                    // プレイヤーに近づく
                    foreach (var loc in Approach(enemy.Loc, playerNextLoc)) {
                        Dir dir = enemy.Loc.Toward(loc);
                        if (!locs[loc.Row, loc.Col] && floor.CanAdvance(enemy.Loc, dir)) {
                            q.Add(new ActEnemyMove(enemy, loc));
                            locs[loc.Row, loc.Col] = true;
                            locs[enemy.Row, enemy.Col] = false;
                            updated = true;
                            used[i] = true;
                            break;
                        }
                    }
                }
                else { // プレイヤーが視界にいない
                    // 巡回モード

                    if (enemy.IsLockedOn) {
                         foreach (var loc in Approach(enemy.Loc, enemy.Target)) {
                            Dir dir = enemy.Loc.Toward(loc);
                            if (!locs[loc.Row, loc.Col] && floor.CanAdvance(enemy.Loc, dir)) {
                                q.Add(new ActEnemyMove(enemy, loc));
                                locs[loc.Row, loc.Col] = true;
                                locs[enemy.Row, enemy.Col] = false;
                                updated = true;
                                used[i] = true;
                                break;
                            }
                        }
                    }
                    else if (floor.IsPassage(enemy.Loc)) { // 通路にいるなら前進する
                        enemy.CancelTarget();

                        foreach (var loc in Advance(enemy.Loc, enemy.Dir)) {
                            Dir dir = enemy.Loc.Toward(loc);
                            if (!locs[loc.Row, loc.Col] && floor.CanAdvance(enemy.Loc, dir)) {
                                q.Add(new ActEnemyMove(enemy, loc));
                                locs[loc.Row, loc.Col] = true;
                                locs[enemy.Row, enemy.Col] = false;
                                updated = true;
                                used[i] = true;
                                break;
                            }
                        }
                    }
                    else if (floor.IsRoom(enemy.Loc)) { // 部屋にいるなら入り口に向かう
                        Room room = floor.FindRoom(enemy.Loc);
                        Assert.IsTrue(room != null && room.Entrances.Length > 0);

                        Loc[] entrances = room.Entrances;
                        if (entrances.Length > 1) {
                            // 隣接する入り口には向かわない(通路から部屋に入って、また通路に戻るパターン)
                            entrances = entrances.Where(e => !e.IsNeighbor(enemy.Loc)).ToArray();
                        }

                        Loc target = Utils.Choice(entrances);
                        enemy.LockOn(target);
                        foreach (var loc in Approach(enemy.Loc, enemy.Target)) {
                            Dir dir = enemy.Loc.Toward(loc);
                            if (!locs[loc.Row, loc.Col] && floor.CanAdvance(enemy.Loc, dir)) {
                                q.Add(new ActEnemyMove(enemy, loc));
                                locs[loc.Row, loc.Col] = true;
                                locs[enemy.Row, enemy.Col] = false;
                                updated = true;
                                used[i] = true;
                                break;
                            }
                        }
                    }
                }
            }
        }

        // 攻撃(移動以外)するキャラ
        for (int i = 0; i < enemies.Count; i++) {
            if (used[i]) continue;
            if (CanDirectAttack(enemies[i].Loc, playerNextLoc, floor)) {
                q.Add(new ActEnemyAttack(enemies[i], player, playerNextLoc));
                used[i] = true;
            }
        }

        // 移動も行動も出来ないキャラは待機
        for (int i = 0; i < enemies.Count; i++) {
            if (used[i]) continue;
            // q.Add(new ActEnemyWait(enemies[i]));
            used[i] = true;
        }

        return q;
    }

    public static List<Act> Detect2(List<Enemy> enemies, Player player, Loc playerNextLoc, Floor floor) {

        // 敵をプレイヤーに近い距離順にソートする
        // TODO:プレイヤーから逃げる行動をとる場合は、プレイヤーから遠い順に行動を決める
        enemies.Sort((a, b) => {
            var x = a.Loc.SquareDistance(playerNextLoc);
            var y = b.Loc.SquareDistance(playerNextLoc);
            return x.CompareTo(y);
        });

        var q = new List<Act>();

        var locs = new bool[floor.Rows, floor.Cols]; // キャラクターの位置
        var used = new bool[enemies.Count];
        for (int i = 0; i < enemies.Count; i++) {
            if (enemies[i].ActCount <= 0) { // 行動済み
                used[i] = true; // 行動済み
            }
            else if (enemies[i].IsBehavioralIncapacitation()) {
                used[i] = true;
            }
            Loc loc = enemies[i].Loc;
            locs[loc.Row, loc.Col] = true;
        }
        locs[playerNextLoc.Row, playerNextLoc.Col] = true;

        // 移動するキャラ
        bool updated = true;
        while (updated) {
            updated = false;
            for (int i = 0; i < enemies.Count; i++) {
                if (used[i]) continue;

                var enemy = enemies[i];
                Assert.IsTrue(locs[enemy.Loc.Row, enemy.Loc.Col]);


                { // プレイヤーが視界にいない
                    // 巡回モード

                    if (enemy.IsLockedOn) {
                         foreach (var loc in Approach(enemy.Loc, enemy.Target)) {
                            Dir dir = enemy.Loc.Toward(loc);
                            if (!locs[loc.Row, loc.Col] && floor.CanAdvance(enemy.Loc, dir)) {
                                q.Add(new ActEnemyMove(enemy, loc));
                                locs[loc.Row, loc.Col] = true;
                                locs[enemy.Row, enemy.Col] = false;
                                updated = true;
                                used[i] = true;
                                break;
                            }
                        }
                    }
                    else if (floor.IsPassage(enemy.Loc)) { // 通路にいるなら前進する
                        enemy.CancelTarget();

                        foreach (var loc in Advance(enemy.Loc, enemy.Dir)) {
                            Dir dir = enemy.Loc.Toward(loc);
                            if (!locs[loc.Row, loc.Col] && floor.CanAdvance(enemy.Loc, dir)) {
                                q.Add(new ActEnemyMove(enemy, loc));
                                locs[loc.Row, loc.Col] = true;
                                locs[enemy.Row, enemy.Col] = false;
                                updated = true;
                                used[i] = true;
                                break;
                            }
                        }
                    }
                    else if (floor.IsRoom(enemy.Loc)) { // 部屋にいるなら入り口に向かう
                        Room room = floor.FindRoom(enemy.Loc);
                        Assert.IsTrue(room != null && room.Entrances.Length > 0);

                        Loc[] entrances = room.Entrances;
                        if (entrances.Length > 1) {
                            // 隣接する入り口には向かわない(通路から部屋に入って、また通路に戻るパターン)
                            entrances = entrances.Where(e => !e.IsNeighbor(enemy.Loc)).ToArray();
                        }

                        Loc target = Utils.Choice(entrances);
                        enemy.LockOn(target);
                        foreach (var loc in Approach(enemy.Loc, enemy.Target)) {
                            Dir dir = enemy.Loc.Toward(loc);
                            if (!locs[loc.Row, loc.Col] && floor.CanAdvance(enemy.Loc, dir)) {
                                q.Add(new ActEnemyMove(enemy, loc));
                                locs[loc.Row, loc.Col] = true;
                                locs[enemy.Row, enemy.Col] = false;
                                updated = true;
                                used[i] = true;
                                break;
                            }
                        }
                    }
                }
            }
        }

        // 移動も行動も出来ないキャラは待機
        for (int i = 0; i < enemies.Count; i++) {
            if (used[i]) continue;
            // q.Add(new ActEnemyWait(enemies[i]));
            used[i] = true;
        }

        return q;
    }

}
