using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public static class EnemyStrategy {
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
            var x = a.SqDistance(to);
            var y = b.SqDistance(to);
            return x.CompareTo(y);
        });
        return locs;
    }

    private static bool CanDirectAttack(Loc fm, Loc to, Map map) {
        if (fm.IsNeighbor(to)) {
            Dir dir = fm.Toward(to);
            return map.CanAdvance(fm, dir);
        }
        return false;
    }

    public static List<Act> Detect(List<Enemy> enemies, Player player, Loc playerNextLoc, Map map) {
        // 敵をプレイヤーに近い距離順にソートする
        enemies.Sort((a, b) => {
            var x = a.Loc.SqDistance(playerNextLoc);
            var y = b.Loc.SqDistance(playerNextLoc);
            return x.CompareTo(y);
        });

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

        // 移動するキャラ
        bool updated = true;
        while (updated) {
            updated = false;
            for (int i = 0; i < enemies.Count; i++) {
                if (used[i]) continue;

                var enemy = enemies[i];
                Assert.IsTrue(locs[enemy.Loc.Row, enemy.Loc.Col]);

                if (CanDirectAttack(enemy.Loc, playerNextLoc, map)) continue;

                // プレイヤーに近づく
                foreach (var loc in Approach(enemy.Loc, playerNextLoc)) {
                    Dir dir = enemy.Loc.Toward(loc);
                    // Debug.LogFormat("チェック: {0} : {1}", loc, enemy);
                    if (!locs[loc.Row, loc.Col] && map.CanAdvance(enemy.Loc, dir)) {
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

        if (q.Count > 0) return q;

        // 攻撃(移動以外)するキャラ
        for (int i = 0; i < enemies.Count; i++) {
            if (used[i]) continue;
            if (CanDirectAttack(enemies[i].Loc, playerNextLoc, map)) {
                q.Add(new ActEnemyAttack(enemies[i], player));
                used[i] = true;
            }
        }

        return q;
    }
}
