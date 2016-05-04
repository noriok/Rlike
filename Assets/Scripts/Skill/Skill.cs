using UnityEngine;
using System.Collections;

public abstract class Skill {

    public virtual IEnumerator Use(CharacterBase sender, MainSystem sys) {
        yield break;
    }

    public virtual IEnumerator Hit(CharacterBase sender, CharacterBase target, MainSystem sys) {
        yield break;
    }

}
