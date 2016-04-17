using System.Collections;
using UnityEngine;
// using System.Collections;

public class ActPlayerAttack : Act {
    private CharacterBase _target;

    public ActPlayerAttack(Player player, CharacterBase target) : base(player) {
        _target = target;
    }

    private IEnumerator Anim() {
        // 攻撃キャラを、ターゲットの中間に移動させる(攻撃アニメーションの代替)
        var src = Actor.Position;
        var dst = src;
        dst.x = (src.x + _target.Position.x) / 2;
        dst.y = (src.y + _target.Position.y) / 2;
        Actor.Position = dst;

        var dmg = new System.Random().Next(60);
        _target.DamageHp(dmg);

        var pos = _target.Position;
        pos.y -= 0.09f;
        yield return EffectAnim.PopupWhiteDigits(dmg, pos, () => {
            Actor.Position = src;
        });

        if (_target.Hp <= 0) { // 敵を倒したときに爆発アニメーション
            var obj = Resources.Load("Prefabs/Animations/dead");
            var gobj = (GameObject)GameObject.Instantiate(obj, _target.Position, Quaternion.identity);
            _target.Destroy();
            while (gobj != null) {
                yield return null;
            }
        }
        AnimationFinished = true;
    }

    public override void RunAnimation(MainSystem sys) {
        if (_target == null) {
            AnimationFinished = true;
        }
        else {
            sys.StartCoroutine(Anim());
        }
    }

    public override void RunEffect(MainSystem sys) {
        DLog.D("{0} attack target:{1}", Actor, _target);
    }
}
