// using UnityEngine;
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
        DLog.D("{0} attack --> {1}", Actor, _target);
    }
}
