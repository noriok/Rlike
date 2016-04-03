// using UnityEngine;
// using System.Collections;

public class ActEnemyAttack : Act {
    private CharacterBase _target;

    public ActEnemyAttack(Enemy enemy, CharacterBase target) : base(enemy) {
        _target = target;
    }

    protected override int GetPriority() {
        return ActPriority.Attack;
    }

    public override void RunAnimation(MainSystem sys) {

    }

    public override void RunEffect(MainSystem sys) {
        // Actor が _target に攻撃

    }
}
