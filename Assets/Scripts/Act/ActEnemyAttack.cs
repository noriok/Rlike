using UnityEngine;
// using System.Collections;

public class ActEnemyAttack : Act {
    private CharacterBase _target;

    public ActEnemyAttack(Enemy enemy, CharacterBase target) : base(enemy) {
        _target = target;
    }

    public override void RunAnimation(MainSystem sys) {
        AnimationFinished = true;
    }

    public override void RunEffect(MainSystem sys) {
        // Actor が _target に攻撃

        Debug.Log(string.Format("enemy attack Self:{0}(Loc:{1}) Target:{2}(Loc:{3})", Actor, Actor.Loc, _target, _target.Loc));
    }
}
