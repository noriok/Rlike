﻿// using UnityEngine;
// using System.Collections;

public class ActPlayerAttack : Act {
    private CharacterBase _target;

    public ActPlayerAttack(Player player, CharacterBase target) : base(player) {
        _target = target;
    }

    public override void RunAnimation(MainSystem sys) {
        if (_target == null) {
            AnimationFinished = true;
        }
        else {
            // 攻撃キャラを、ターゲットの中間に移動させる(攻撃アニメーションの代替)
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
    }

    public override void RunEffect(MainSystem sys) {
        DLog.D("{0} attack target:{1}", Actor, _target);
    }
}
