// using UnityEngine;
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
            var dmg = new System.Random().Next(30);
            var pos = _target.Position;
            pos.y -= 0.09f;
            sys.StartCoroutine(EffectAnim.PopupWhiteDigits(dmg, pos, () => AnimationFinished = true));
        }
    }

    public override void RunEffect(MainSystem sys) {
        DLog.D("{0} attack target:{1}", Actor, _target);
    }
}
