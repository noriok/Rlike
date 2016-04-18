using UnityEngine;
using System.Collections;

public class ActTrapHeal : Act {

	public ActTrapHeal(CharacterBase target) : base(target) {

	}

	public override bool IsTrapAct() {
		return true;
	}

	private IEnumerator Anime() {
		yield return EffectAnim.Heal(Actor);

		var healHp = 10;
		yield return EffectAnim.PopupGreenDigits(Actor, healHp);
		AnimationFinished = true;
	}

	public override void RunAnimation(MainSystem sys) {
		sys.StartCoroutine(Anime());
	}

	public override void RunEffect(MainSystem sys) {
		DLog.D("Trap heal {0}", Actor);
	}
}
