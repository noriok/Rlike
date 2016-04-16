// using UnityEngine;
// using System.Collections;

public class ActPlayerAttack : Act {
    private CharacterBase _target;

    public ActPlayerAttack(Player player, CharacterBase target) : base(player) {
        _target = target;
    }

    public override void RunAnimation(MainSystem sys) {
        AnimationFinished = true;
    }

    public override void RunEffect(MainSystem sys) {
        // Debug.LogFormat("@@@ player attack target:{0}", _target);
        DLog.D("{0} attack target:{1}", Actor, _target);
    }
}
