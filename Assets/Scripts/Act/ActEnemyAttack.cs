using UnityEngine;
// using System.Collections;

public class ActEnemyAttack : Act {
    private CharacterBase _target;

    public ActEnemyAttack(Enemy enemy, CharacterBase target) : base(enemy) {
        _target = target;
    }

    public override void RunAnimation(MainSystem sys) {
        // 攻撃キャラを、ターゲットとの中間に移動させる(攻撃アニメーションの代替)
        var src = Actor.Position;
        var dst = src;
        dst.x = (src.x + _target.Position.x) / 2;
        dst.y = (src.y + _target.Position.y) / 2;
        Actor.Position = dst;

        var dmg = new System.Random().Next(30);
        var pos = _target.Position;
        pos.y -= 0.09f;
        sys.StartCoroutine(EffectAnim.PopupWhiteDigits(dmg, pos, () => {
            Actor.Position = src;
            AnimationFinished = true;
        }));
    }

    public override void RunEffect(MainSystem sys) {
        // Actor が _target に攻撃
        DLog.D("{0} attack --> {1}", Actor, _target);
    }
}
