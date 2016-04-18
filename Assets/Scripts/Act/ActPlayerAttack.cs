using System.Collections;
// using UnityEngine;

public class ActPlayerAttack : Act {
    private CharacterBase _target;

    public ActPlayerAttack(Player player, CharacterBase target) : base(player) {
        _target = target;
    }

    protected override IEnumerator RunAnimation(MainSystem sys) {
        // 攻撃キャラを、ターゲットの中間に移動させる(攻撃アニメーションの代替)
        var src = Actor.Position;
        var dst = src;
        dst.x = (src.x + _target.Position.x) / 2;
        dst.y = (src.y + _target.Position.y) / 2;
        Actor.Position = dst;

        var dmg = new System.Random().Next(60);
        _target.DamageHp(dmg);
        yield return EffectAnim.PopupWhiteDigits(_target, dmg);
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
