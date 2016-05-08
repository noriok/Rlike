using UnityEngine;
using System.Collections;

public abstract class Skill {

    public virtual IEnumerator Use(CharacterBase sender, MainSystem sys) {
        Debug.Log("Skill.Use 効果なし");
        yield break;
    }

    // キャラクターにヒット
    public virtual IEnumerator Hit(CharacterBase sender, CharacterBase target, MainSystem sys) {
        Debug.Log("Skill.Hit(キャラクター) 効果なし");
        yield break;
    }

    // 障害物にヒット
    public virtual IEnumerator Hit(CharacterBase sender, Loc target, MainSystem sys) {
        Debug.Log("Skill.Hit(障害物) 効果なし");
        yield break;
    }

}
