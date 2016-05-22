using UnityEngine;
using System.Collections;

public class SkillFreeze : Skill {

    public override IEnumerator Hit(CharacterBase sender, CharacterBase target, MainSystem sys) {
        target.AddStatus(StatusType.Freeze);
        yield return null;
    }
}
