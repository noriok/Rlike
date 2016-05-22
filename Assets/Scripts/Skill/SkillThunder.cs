using UnityEngine;
using System.Collections;

public class SkillThunder : Skill {

    public override IEnumerator Hit(CharacterBase sender, CharacterBase target, MainSystem sys) {
        yield return EffectAnim.Thunder(target.Position);

        int damage = 99;
        yield return Anim.Par(sys,
                              () => target.DamageAnim(damage),
                              () => EffectAnim.PopupWhiteDigits(target, damage));
        target.DamageHp(damage);

        if (target.Hp <= 0) {
            var pos = target.Position;
            target.Destroy();
            yield return EffectAnim.Dead(pos);
        }
    }
}
