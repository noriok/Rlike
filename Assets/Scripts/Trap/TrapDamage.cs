using UnityEngine;
using System.Collections;

public class TrapDamage : Trap {
	public TrapDamage(Loc loc, GameObject gobj) : base(loc, gobj) {

	}

	public override IEnumerator RunAnimation(CharacterBase sender, MainSystem sys) {
		var dmg = 10 + new System.Random().Next(5);
		yield return EffectAnim.PopupWhiteDigits(sender, dmg);
	}

    public override string Name() {
        return "ダメージのワナ";
    }

    public override string Description() {
        return "踏むとダメージを受けるぞ。";
    }

    public override string ImagePath() {
        return "Images/trap3";
    }
}
