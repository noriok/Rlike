using System;
using System.Collections;
// using UnityEngine;

public class ActPlayerAttack : Act {
    private CharacterBase _target;

    public ActPlayerAttack(Player player, CharacterBase target) : base(player) {
        _target = target;
    }

    protected override IEnumerator Run(MainSystem sys) {
        // 攻撃キャラを、ターゲットの中間に移動させる(攻撃アニメーションの代替)
        var src = Actor.Position;
        var dst = src;
        dst.x = (src.x + _target.Position.x) / 2;
        dst.y = (src.y + _target.Position.y) / 2;
        Actor.Position = dst;

        var rand = new Random();
        var dmg = 20 + rand.Next(-2, 5);

        _target.RemoveStatus(StatusType.Sleep);
        // TODO:Func<IEnumerator> ではなくて、IEnumerator を渡す
        yield return Anim.Par(sys,
                              () => _target.DamageAnim(dmg),
                              () => EffectAnim.PopupWhiteDigits(_target, dmg));
        _target.DamageHp(dmg);
        Actor.Position = src;

        if (_target.Hp <= 0) { // 敵を倒したときに爆発アニメーション
            var pos = _target.Position;
            _target.Destroy();
            yield return EffectAnim.Dead(pos);
        }
    }

    public override void Apply(MainSystem sys) {
        DLog.D("{0} attack --> {1}", Actor, _target);
    }
}
