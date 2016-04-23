using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class ActPlayerUseItem : Act {

	public ActPlayerUseItem(Player player) : base(player) {
	}

	protected override IEnumerator RunAnimation(MainSystem sys) {
		yield return EffectAnim.Heal(Actor.Position);

		var healHp = new System.Random().Next(29) + 1;
		yield return EffectAnim.PopupGreenDigits(Actor, healHp);
	}

	public override void Apply(MainSystem sys) {
		DLog.D("{0} item", Actor);
	}
}
