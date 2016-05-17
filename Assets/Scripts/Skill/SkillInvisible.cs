// using UnityEngine;
using System.Collections;

public class SkillInvisible : Skill {

    public override IEnumerator Use(CharacterBase sender, MainSystem sys) {
        sender.AddStatus(Status.Invisible, 20);
        yield return null;
    }

    public override IEnumerator Hit(CharacterBase sender, CharacterBase target, MainSystem sys) {
        target.AddStatus(Status.Invisible, 20);
        yield return null;
    }
}
