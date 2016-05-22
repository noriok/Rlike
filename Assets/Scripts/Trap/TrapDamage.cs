using UnityEngine;
using System.Collections;

public class TrapDamage : Trap {
	public TrapDamage(Loc loc, GameObject gobj) : base(loc, gobj) {

	}

	public override IEnumerator RunAnimation(CharacterBase sender, MainSystem sys) {
		var dmg = 10 + new System.Random().Next(5);
		yield return EffectAnim.PopupWhiteDigits(sender, dmg);
	}
}
