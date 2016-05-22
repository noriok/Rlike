using UnityEngine;
using System.Collections;

// 引数で効果範囲を指定する
public class SkillSleep : Skill {

    public override IEnumerator Use(CharacterBase sender, MainSystem sys) {
        sender.AddStatus(StatusType.Sleep, 7);
        yield return null;
    }

    public override IEnumerator Hit(CharacterBase sender, CharacterBase target, MainSystem sys) {
        target.AddStatus(StatusType.Sleep, 7);
        yield return null;
    }
}
