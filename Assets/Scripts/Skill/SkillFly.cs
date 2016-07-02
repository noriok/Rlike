using System.Collections;

// とびつき TODO:rename
public class SkillFly : Skill {

    private IEnumerator Run(Player player, Loc to, MainSystem sys) {
        yield return CAction.MovePlayer(player, to);

        // とびついた先が水ならワープ
        if (sys.IsWater(player.Loc)) {
            yield return new SkillWarp().Use(player, sys);
        }
    }

    public override IEnumerator Hit(CharacterBase sender, CharacterBase target, MainSystem sys) {
        Assert.IsTrue(sender is Player);
        // ターゲットの 1 歩前まで移動する
        Loc to = target.Loc.Backward(sender.Dir);
        yield return Run((Player)sender, to, sys);
    }

    public override IEnumerator Hit(CharacterBase sender, Loc target, MainSystem sys) {
        Assert.IsTrue(sender is Player);
        // ターゲットの 1 歩前まで移動する
        Loc to = target.Backward(sender.Dir);
        yield return Run((Player)sender, to, sys);
    }
}
