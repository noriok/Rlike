// using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class ActEnemyMove : Act {
    private int _drow;
    private int _dcol;

    // nextLoc = 移動後の位置
    public ActEnemyMove(Enemy enemy, Loc nextLoc) : base(enemy) {
        Assert.IsTrue(enemy.Loc.IsNeighbor(nextLoc));

        _drow = nextLoc.Row - enemy.Row;
        _dcol = nextLoc.Col - enemy.Col;
    }

    public override bool IsMoveAct() {
        return true;
    }

    protected override IEnumerator RunAnimation(MainSystem sys) {
        Actor.ChangeDir(Utils.ToDir(_drow, _dcol));
        yield return CAction.Walk(Actor, _drow, _dcol, null);
    }

    public override void Apply(MainSystem sys) {
        var nextLoc = Actor.Loc + new Loc(_drow, _dcol);
        DLog.D("{0} move {1} -> {2}", Actor, Actor.Loc, nextLoc);
        Actor.UpdateLoc(nextLoc);
    }
}
