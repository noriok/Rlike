﻿using UnityEngine;
// using System.Collections;

public class ActEnemyAttack : Act {
    private CharacterBase _target;

    public ActEnemyAttack(Enemy enemy, CharacterBase target) : base(enemy) {
        _target = target;
    }

    protected override int GetPriority() {
        return ActPriority.EnemyAttack;
    }

    public override void RunAnimation(MainSystem sys) {
        AnimationFinished = true;
    }

    public override void RunEffect(MainSystem sys) {
        // Actor が _target に攻撃

        Debug.Log(string.Format("enemy attack Self:{0} Target:{1}", Actor, _target));
    }
}
