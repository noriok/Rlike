using UnityEngine;
using System.Collections;

public class SkillHeal : Skill {

    public override IEnumerator Use(CharacterBase sender, MainSystem sys) {
        yield return EffectAnim.Heal(sender.Position);
        var healHp = 30;
        yield return EffectAnim.PopupGreenDigits(sender, healHp);
    }

    public override IEnumerator Hit(CharacterBase sender, CharacterBase target, MainSystem sys) {
        return Use(target, sys);
    }

}
