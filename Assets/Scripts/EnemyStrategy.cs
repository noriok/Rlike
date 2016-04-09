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

    // loc から dir 方向へ進んだ Loc を返す
    private static Loc Forward(Loc loc, Dir dir) {
        int drow = 0;
        int dcol = 0;
        switch (dir) {
        case Dir.N:  drow = -1; dcol =  0; break;
        case Dir.NE: drow = -1; dcol =  1; break;
        case Dir.E:  drow =  0; dcol =  1; break;
        case Dir.SE: drow =  1; dcol =  1; break;
        case Dir.S:  drow =  1; dcol =  0; break;
        case Dir.SW: drow =  1; dcol = -1; break;
        case Dir.W:  drow =  0; dcol = -1; break;
        case Dir.NW: drow = -1; dcol = -1; break;
        }
        return new Loc(loc.Row + drow, loc.Col + dcol);
    }

    // fm から to に向かうために進むべき方向を返す
    private static Dir Toward(Loc fm, Loc to) {
        Loc delta = to - fm;
        int drow = delta.Row;
        int dcol = delta.Col;

        if (drow != 0) drow /= Math.Abs(drow);
        if (dcol != 0) dcol /= Math.Abs(dcol);

        Assert.IsFalse(drow == 0 && dcol == 0);
        if (drow == -1) {
            switch (dcol) {
            case -1: return Dir.NW;
            case  0: return Dir.N;
            case  1: return Dir.NE;
            }
        }
        else if (drow == 0) {
            switch (dcol) {
            case -1: return Dir.W;
            case  0: Assert.IsTrue(false); break;
            case  1: return Dir.E;
            }
        }
        else if (drow == 1) {
            switch (dcol) {
                case -1: return Dir.SW;
                case  0: return Dir.S;
                case  1: return Dir.SE;
            }
        }

        Assert.IsTrue(false);
        return Dir.N; // ここには到達しない
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

                if (enemy.Loc.IsNeighbor(playerNextLoc)) continue;

                // プレイヤーに近づく

                Dir dir = Toward(enemy.Loc, playerNextLoc); // プレイヤーの方向
                Dir[] dirs = {
                    dir, // プレイヤー進行方向
                    // TODO:プレイヤーに近づく方向に進むように。
                    dir.Clockwise(), // 進行方向に進めないなら、斜め進行方向に進めないか試す
                    dir.Anticlockwise(),
                };

                foreach (Dir d in dirs) {
                    Loc to = Forward(enemy.Loc, d);
                    Debug.LogFormat("enemy.Loc:{0} playerNextLoc:{1} to:{2}", enemy.Loc, playerNextLoc, to);
                    // Debug.LogFormat("to: {0} enemy: {1} playerNextLoc: {2}", to, enemy.Loc, playerNextLoc);
                    if (!locs[to.Row, to.Col]) { // 空いているなら、移動する
                        q.Add(new ActEnemyMove(enemy, to));
                        Debug.LogFormat("enemy {0} -> {1}", enemy.Loc, to);

                        locs[to.Row, to.Col] = true;
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

            if (CanAttack(enemies[i].Loc, playerNextLoc)) {
                Debug.LogFormat("隣接しているので攻撃 {0} {1}", enemies[i].Loc, playerNextLoc);
                q.Add(new ActEnemyAttack(enemies[i], player));
                used[i] = true;
            }
        }

        return q;
    }
}
