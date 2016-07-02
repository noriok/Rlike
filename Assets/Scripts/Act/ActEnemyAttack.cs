// using System;
using System.Collections;
using UnityEngine;

public class ActEnemyAttack : Act {
    private CharacterBase _target;
    // プレイヤー移動直後にワープした場合、攻撃対象が既にいない可能性がある。
    // 攻撃前にプレイヤーの座標が変わっていないかチェックして、もしいないなら行動を決定し直す
    private Loc _targetLoc;
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

    protected override IEnumerator Run(MainSystem sys) {
       // 攻撃キャラを、ターゲットとの中間に移動させる(攻撃アニメーションの代替)
        var src = Actor.Position;
        var dst = src;
        dst.x = (src.x + _target.Position.x) / 2;
        dst.y = (src.y + _target.Position.y) / 2;
        Actor.Position = dst;

        // ターゲットの方を向く
        Actor.UpdateDir(Actor.Loc.Toward(_target.Loc));
        _target.HideDirection();

        // ターゲットは攻撃者の方を向く。ただし以下の場合は振り向かない
        // - 目の前に敵が存在する
        // - 攻撃者が透明状態
        if (!sys.ExistsEnemy(_target.Front()) && !Actor.IsInvisible()) {
            _target.UpdateDir(_target.Loc.Toward(Actor.Loc));
        }

        _target.RemoveStatus(StatusType.Sleep);
        var dmg = 16 + new System.Random().Next(30);
        yield return Anim.Par(sys,
                              () => _target.DamageAnim(dmg),
                              () => EffectAnim.PopupWhiteDigits(_target, dmg));

        _target.DamageHp(dmg);
        Actor.Position = src;
    }

    public override void OnFinished(MainSystem sys) {
        // Actor が _target に攻撃
        DLog.D("{0} attack --> {1}", Actor, _target);
    }
}
