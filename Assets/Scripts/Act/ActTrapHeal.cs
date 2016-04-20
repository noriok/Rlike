// using UnityEngine;
using System.Collections;

public class ActTrapHeal : ActTrap {

	public ActTrapHeal(CharacterBase target) : base(target) {
	}

	protected override IEnumerator RunAnimation(MainSystem sys) {
		yield return EffectAnim.Heal(Actor);

		var healHp = 10;
		yield return EffectAnim.PopupGreenDigits(Actor, healHp);
	}

	public override void Apply(MainSystem sys) {
		DLog.D("Trap heal {0}", Actor);
	}
}
