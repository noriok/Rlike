// using UnityEngine;
using System.Collections;

public class ActTrap : Act {
    private Trap _trap;

    public ActTrap(CharacterBase sender, Trap trap) : base(sender) {
        _trap = trap;
    }

    public override bool IsTrapAct() {
        return true;
    }

    protected override IEnumerator Run(MainSystem sys) {
        Actor.HideDirection();
        _trap.Visible = true;
        yield return _trap.RunAnimation(Actor, sys);
    }

    public override void OnFinished(MainSystem sys) {
        DLog.D("Trap {0}", Actor);
    }
}
