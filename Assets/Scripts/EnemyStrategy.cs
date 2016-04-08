﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public static class EnemyStrategy {

    // 隣接しているか
    private static bool IsNeighbor(Loc a, Loc b) {
        if (Math.Abs(a.Row - b.Row) <= 1 &&
            Math.Abs(a.Col - b.Col) <= 1)
        {
            return true;
        }
        return false;
    }

    private static Loc Toward(Loc fm, Loc to) {
        int drow = to.Row - fm.Row;
        int dcol = to.Col - fm.Col;

        if (drow != 0) drow /= Math.Abs(drow);
        if (dcol != 0) dcol /= Math.Abs(dcol);
        return new Loc(drow, dcol);
    }

    public static Act Simple(Enemy enemy, Player player, Loc playerNextLoc) {
        if (IsNeighbor(enemy.Loc, playerNextLoc)) {
            return new ActEnemyAttack(enemy, player);
        }

        Loc delta = Toward(enemy.Loc, playerNextLoc);
        Debug.Log(string.Format("delta {0}", delta));
        return new ActEnemyMove(enemy, delta.Row, delta.Col);

        // 移動できないなら待機
//        // 待機
//        return new ActEnemyWait(enemy);
    }


    // TODO: 移動するキャラがいるなら、そのキャラを全て返す
    //       移動するキャラがいないなら、移動以外の行動を行うキャラを 1 体返す
    public static List<Act> Detect(List<Enemy> enemies, Player player, Loc playerNextLoc) {
        var q = new List<Act>();

        var locs = new bool[200, 200];
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

                if (IsNeighbor(enemy.Loc, playerNextLoc)) continue;

                // プレイヤーに近づく
                Loc delta = Toward(enemy.Loc, playerNextLoc);
                Loc to = enemy.Loc + delta;
                // Debug.LogFormat("to: {0} enemy: {1} playerNextLoc: {2}", to, enemy.Loc, playerNextLoc);
                if (!locs[to.Row, to.Col]) { // 空いているなら、移動する
                    q.Add(new ActEnemyMove(enemy, delta.Row, delta.Col));
                    Debug.LogFormat("enemy {0} -> {1}", enemy.Loc, to);

                    locs[to.Row, to.Col] = true;
                    locs[enemy.Row, enemy.Col] = false;
                    updated = true;
                    used[i] = true;
                    break;
                }
            }
        }

        if (q.Count > 0) return q;

        // 攻撃(移動以外)するキャラ
        for (int i = 0; i < enemies.Count; i++) {
            if (used[i]) continue;

            if (IsNeighbor(enemies[i].Loc, playerNextLoc)) {
                q.Add(new ActEnemyAttack(enemies[i], player));
                used[i] = true;
            }
        }

        return q;
    }
}
