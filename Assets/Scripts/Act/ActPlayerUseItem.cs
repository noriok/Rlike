using UnityEngine;
using System.Collections;

public class ActPlayerUseItem : Act {

	public ActPlayerUseItem(Player player) : base(player) {

	}

	private IEnumerator Wait() {
		yield return EffectAnim.Heal(Actor);

		var healHp = new System.Random().Next(29) + 1;
		yield return EffectAnim.PopupGreenDigits(Actor, healHp);
		AnimationFinished = true;
	}

	public override void RunAnimation(MainSystem sys) {
		sys.StartCoroutine(Wait());
	}

	public override void RunEffect(MainSystem sys) {
		DLog.D("{0} item", Actor);
	}
}
