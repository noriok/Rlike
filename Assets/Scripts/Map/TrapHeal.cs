using UnityEngine;
using System.Collections;

public class TrapHeal : Trap {

	public TrapHeal(Loc loc, GameObject gobj) : base(loc, gobj) {

	}

	public override IEnumerator RunAnimation(CharacterBase sender, MainSystem sys) {
		yield return EffectAnim.Heal(sender.Position);

		var healHp = 10;
		yield return EffectAnim.PopupGreenDigits(sender, healHp);
	}
}
