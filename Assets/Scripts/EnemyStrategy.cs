using System;
using System.Collections.Generic;
using UnityEngine;

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

    public static List<Act> Detect(List<Enemy> enemies, Player player, Loc playerNextLoc) {
        var q = new List<Act>();

        var used = new bool[enemies.Count];
        for (int i = 0; i < enemies.Count; i++) {
            if (enemies[i].ActCount <= 0) {
                used[i] = true; // 行動済み
            }
        }

        // 敵の移動は、行動出来るものから順番に決めていく
        bool updated = true;
        while (updated) {
            updated = false;

            for (int i = 0; i < enemies.Count; i++) {
                if (used[i]) continue;

                // プレイヤーと隣接しているなら攻撃する
                if (IsNeighbor(enemies[i].Loc, playerNextLoc)) {
                    q.Add(new ActEnemyAttack(enemies[i], player));
                    used[i] = true;
                    updated = true;
                }

                // プレイヤーに近づく

            }
        }

        return q;
    }
}
