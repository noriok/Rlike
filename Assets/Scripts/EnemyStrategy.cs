using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public static class EnemyStrategy {

    // a と b が隣接しているとき、a から b へ攻撃可能か
    private static bool CanAttack(Loc a, Loc b) {
        if (a.IsNeighbor(b)) {
            // TODO:斜めに隣接しているとき、壁が隣接しているなら攻撃不可
            return true;
        }
        return false;
    }

    // TODO: 移動するキャラがいるなら、そのキャラを全て返す
    //       移動するキャラがいないなら、移動以外の行動を行うキャラを 1 体返す
    public static List<Act> Detect(List<Enemy> enemies, Player player, Loc playerNextLoc) {
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
                if (locs[to.Row, to.Col]) { // 進めない
                    // 斜め方向に進めるか試す

                    Loc loc1 = enemy.Loc.Forward(dir.Clockwise());
                    Loc loc2 = enemy.Loc.Forward(dir.Anticlockwise());
                    // 斜め方向いずれも進行できるなら、プレイヤーにもっとも近い方を選択する
                    if (!locs[loc1.Row, loc1.Col] && !locs[loc2.Row, loc2.Col]) {
                        if (loc1.DistanceSq(playerNextLoc) < loc2.DistanceSq(playerNextLoc)) {
                            nextLoc = loc1;
                        }
                        else {
                            nextLoc = loc2;
                        }
                    }
                    else if (!locs[loc1.Row, loc1.Col]) {
                        nextLoc = loc1;
                    }
                    else if (!locs[loc2.Row, loc2.Col]) {
                        nextLoc = loc2;

                    }
                }
                else { // to に進める
                    nextLoc = to;
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

            if (CanAttack(enemies[i].Loc, playerNextLoc)) {
                Debug.LogFormat("隣接しているので攻撃 {0} {1}", enemies[i].Loc, playerNextLoc);
                q.Add(new ActEnemyAttack(enemies[i], player));
                used[i] = true;
            }
        }

        return q;
    }
}
