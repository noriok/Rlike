using System.Collections;

// ふきとばし
public class SkillBlow : Skill {

    public override IEnumerator Hit(CharacterBase sender, CharacterBase target, MainSystem sys) {
        Assert.IsTrue(target is Enemy);

        // target をsender.Dir 方向にふきとばす
        CharacterBase hitTarget;
        Loc to = sys.FindHitTarget(target.Loc, sender.Dir, out hitTarget);

        // hitTarget != null なら他の敵と衝突した。TODO:5 ダメージ与える
        yield return CAction.MoveEnemy((Enemy)target, to);

        // ふきとんだ後の着地点
        Loc loc = to.Backward(sender.Dir);
        target.UpdateLoc(loc);
        // 着地点が水ならワープ
        if (sys.IsWater(loc)) {
            yield return new SkillWarp().Use(target, sys);
        }
    }
}
