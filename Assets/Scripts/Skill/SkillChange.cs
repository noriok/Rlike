using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class SkillChange : Skill {

    public override IEnumerator Hit(CharacterBase sender, CharacterBase target, MainSystem sys) {
        Debug.LogFormat("入れ替え sender:{0} target:{1}", sender, target);

        Assert.IsTrue(sender != null && target != null);
        var loc1 = sender.Loc;
        var loc2 = target.Loc;

        yield return Anim.Par(sys,
            () => EffectAnim.Warp(loc1.ToPosition()),
            () => EffectAnim.Warp(loc2.ToPosition()));

        sender.UpdateLoc(loc2);
        target.UpdateLoc(loc1);
        yield return new WaitForSeconds(0.4f);
    }
}
