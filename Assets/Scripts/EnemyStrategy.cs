using System;
using UnityEngine;

public static class EnemyStrategy {

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

    public static Act Simple(Enemy enemy, Player player) {
        if (IsNeighbor(enemy.Loc, player.Loc)) {
            return new ActEnemyAttack(enemy, player);
        }

        Loc delta = Toward(enemy.Loc, player.Loc);
        Debug.Log(string.Format("delta {0}", delta));
        return new ActEnemyMove(enemy, delta.Row, delta.Col);

//        // 待機
//        return new ActEnemyWait(enemy);
    }
}
