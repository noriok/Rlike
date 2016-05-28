using UnityEngine;
using System.Collections;

public class TrapHeal : Trap {

	public TrapHeal(Loc loc, GameObject gobj) : base(loc, gobj) {
	}

	public override IEnumerator RunAnimation(CharacterBase sender, MainSystem sys) {
		yield return EffectAnim.Heal(sender.Position);

		var healHp = 10 + new System.Random().Next(27);
		yield return Anim.Par(sys,
		                      () => sender.HealAnim(healHp),
							  () => EffectAnim.PopupGreenDigits(sender, healHp));
	    sender.HealHp(healHp);
	}

    public override string Name() {
        return "回復のワナ";
    }

    public override string Description() {
        return "踏むとHPが回復するぞ。";
    }

    public override string ImagePath() {
        return "Images/trap1";
    }
}
