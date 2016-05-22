using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class SkillFly : Skill {

    public override IEnumerator Hit(CharacterBase sender, CharacterBase target, MainSystem sys) {
        Assert.IsTrue(sender is Player);

        // ターゲットの 1 歩前まで移動する
        Loc to = target.Loc.Backward(sender.Dir);
        yield return CAction.MovePlayer((Player)sender, to);

        // とびついた先が水ならワープ
        if (sys.IsWater(sender.Loc)) {
            yield return new SkillWarp().Use(sender, sys);
        }
    }

    public override IEnumerator Hit(CharacterBase sender, Loc target, MainSystem sys) {
        Assert.IsTrue(sender is Player);

        // ターゲットの 1 歩前まで移動する
        Loc to = target.Backward(sender.Dir);
        yield return CAction.MovePlayer((Player)sender, to);

        // とびついた先が水ならワープ
        if (sys.IsWater(sender.Loc)) {
            yield return new SkillWarp().Use(sender, sys);
        }
    }

}
