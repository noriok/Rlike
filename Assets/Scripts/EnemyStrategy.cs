// using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public static class EnemyStrategy {

    // TODO: 移動するキャラがいるなら、そのキャラを全て返す
    //       移動するキャラがいないなら、移動以外の行動を行うキャラを 1 体返す
    public static List<Act> Detect(List<Enemy> enemies, Player player, Loc playerNextLoc, Map map) {
        var q = new List<Act>();

        var locs = new bool[200, 200]; // モンスターの位置
        var used = new bool[enemies.Count];
        for (int i = 0; i < enemies.Count; i++) {
            if (enemies[i].ActCount <= 0) {
                used[i] = true; // 行動済み
            }

            Loc loc = enemies[i].Loc;
            locs[loc.Row, loc.Col] = true;
        }

        // 敵の移動は、行動出来るものから順番に決めていく

        // 移動するキャラ
        bool updated = true;
        while (updated) {
            updated = false;
            for (int i = 0; i < enemies.Count; i++) {
                if (used[i]) continue;

                var enemy = enemies[i];
                Assert.IsTrue(locs[enemy.Loc.Row, enemy.Loc.Col]);

                // TODO:隣接していても、攻撃可能とは限らない。
                // 攻撃できない場合は、移動する
                if (enemy.Loc.IsNeighbor(playerNextLoc)) continue;

                // プレイヤーに近づく
                Loc nextLoc = new Loc(-1, -1);

                Dir dir = enemy.Loc.Toward(playerNextLoc); // プレイヤーの方向
                Loc to = enemy.Loc.Forward(dir);
                if (locs[to.Row, to.Col]) { // 他のキャラがいる
                    // 斜め方向に進めるか試す

                    Dir dir1 = dir.Clockwise();
                    Loc loc1 = enemy.Loc.Forward(dir1);
                    Dir dir2 = dir.Anticlockwise();
                    Loc loc2 = enemy.Loc.Forward(dir2);
                    // 斜め方向いずれも進行できるなら、プレイヤーにもっとも近い方を選択する
                    if (!locs[loc1.Row, loc1.Col] && map.CanAdvance(enemy.Loc, dir1) &&
                        !locs[loc2.Row, loc2.Col] && map.CanAdvance(enemy.Loc, dir2))
                    {
                        if (loc1.DistanceSq(playerNextLoc) < loc2.DistanceSq(playerNextLoc)) {
                            nextLoc = loc1;
                        }
                        else {
                            nextLoc = loc2;
                        }
                    }
                    else if (!locs[loc1.Row, loc1.Col] && map.CanAdvance(enemy.Loc, dir1)) {
                        nextLoc = loc1;
                    }
                    else if (!locs[loc2.Row, loc2.Col] && map.CanAdvance(enemy.Loc, dir2)) {
                        nextLoc = loc2;

                    }
                }
                else { // to にキャラがいない
                    if (map.CanAdvance(enemy.Loc, dir)) {
                        nextLoc = to;
                    }
                }

                if (nextLoc.Row != -1 && nextLoc.Col != -1) {
                    q.Add(new ActEnemyMove(enemy, nextLoc));
                    locs[nextLoc.Row, nextLoc.Col] = true;
                    locs[enemy.Row, enemy.Col] = false;
                    updated = true;
                    used[i] = true;
                }
            }
        }

        if (q.Count > 0) return q;

        // 攻撃(移動以外)するキャラ
        for (int i = 0; i < enemies.Count; i++) {
            if (used[i]) continue;

            if (enemies[i].Loc.IsNeighbor(playerNextLoc)) {
                Loc delta = playerNextLoc - enemies[i].Loc;
                Dir dir = Utils.ToDir(delta.Row, delta.Col);
                if (map.CanAttack(enemies[i].Loc, dir)) {
                    q.Add(new ActEnemyAttack(enemies[i], player));
                    used[i] = true;
                }
            }
        }

        return q;
    }
}
