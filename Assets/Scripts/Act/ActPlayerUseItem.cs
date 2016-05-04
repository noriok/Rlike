using UnityEngine;
// using UnityEngine.Assertions;
using System.Collections;

public class ActPlayerUseItem : Act {
    private Item _item;

	public ActPlayerUseItem(Player player, Item item) : base(player) {
        _item = item;
	}

	protected override IEnumerator RunAnimation(MainSystem sys) {
		yield return EffectAnim.Heal(Actor.Position);

		var healHp = new System.Random().Next(29) + 1;
		yield return EffectAnim.PopupGreenDigits(Actor, healHp);
	}

	public override void Apply(MainSystem sys) {
		DLog.D("{0} item", Actor);

        Debug.LogFormat("item:{0} を使いました", _item);
	}
}
