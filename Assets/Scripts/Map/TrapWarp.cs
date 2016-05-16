using UnityEngine;
using System.Collections;

public class TrapWarp : Trap {
    public TrapWarp(Loc loc, GameObject gobj) : base(loc, gobj) {

    }

    public override IEnumerator RunAnimation(CharacterBase sender, MainSystem sys) {
        yield return new SkillWarp().Use(sender, sys);
    }
}
