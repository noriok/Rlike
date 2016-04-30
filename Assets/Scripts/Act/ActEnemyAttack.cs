// using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class ActEnemyAttack : Act {
    private CharacterBase _target;
    private Loc _targetLoc; // 攻撃する前にターゲットが移動する可能性があるので位置を保存

    public ActEnemyAttack(Enemy enemy, CharacterBase target, Loc targetLoc) : base(enemy) {
        Assert.IsTrue(target is Player);
        _target = target;
        _targetLoc = targetLoc;
    }

    public override bool IsInvalid() {
        if (_targetLoc != _target.Loc) {
            Debug.LogFormat("位置が変わりました {0} -> {1}", _targetLoc, _target.Loc);
            return true;
        }
        return false;
    }

    protected override IEnumerator RunAnimation(MainSystem sys) {
       // 攻撃キャラを、ターゲットとの中間に移動させる(攻撃アニメーションの代替)
        var src = Actor.Position;
        var dst = src;
        dst.x = (src.x + _target.Position.x) / 2;
        dst.y = (src.y + _target.Position.y) / 2;
        Actor.Position = dst;

        // ターゲットの方を向く
        Actor.ChangeDir(Actor.Loc.Toward(_target.Loc));

        // ターゲットは攻撃者の方を向く。ただし既に敵の方を向いている場合は向きは変えない
        if (!sys.ExistsEnemy(_target.Front())) {
            _target.ChangeDir(_target.Loc.Toward(Actor.Loc));
        }

        _target.RemoveStatus(Status.Sleep);
        var dmg = 1 + new System.Random().Next(30);
        // yield return _target.DamageAnim(dmg);
        yield return Anim.Par(sys,
                              () => _target.DamageAnim(dmg),
                              () => EffectAnim.PopupWhiteDigits(_target, dmg));

        _target.DamageHp(dmg);
        // yield return EffectAnim.PopupWhiteDigits(_target, dmg);
        Actor.Position = src;
    }

    public override void Apply(MainSystem sys) {
        // Actor が _target に攻撃
        DLog.D("{0} attack --> {1}", Actor, _target);
    }
}
