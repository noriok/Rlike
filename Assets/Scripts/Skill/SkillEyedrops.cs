using UnityEngine;
using System.Collections;

public class SkillEyedrops : Skill {

    public override IEnumerator Use(CharacterBase sender, MainSystem sys) {
        sys.Eyedrops();
        yield return null;
    }
}
