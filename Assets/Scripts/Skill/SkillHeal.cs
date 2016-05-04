using UnityEngine;
using System.Collections;

public class SkillHeal : Skill {

    public override IEnumerator Use(CharacterBase sender, MainSystem sys) {
        yield return EffectAnim.Heal(sender.Position);

        var healHp = 30;
        yield return EffectAnim.PopupGreenDigits(sender, healHp);
    }
}
