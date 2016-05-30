using UnityEngine;
using System.Collections;

public class SkillWarp : Skill {

    public override IEnumerator Use(CharacterBase sender, MainSystem sys) {
        var src = sender.Position;
        float duration = 0.5f;
        float elapsed = 0;
        while (elapsed <= duration) {
            float y = UTween.Ease(EaseType.InCubic, src.y, src.y + 6, elapsed / duration);
            sender.Position = new Vector3(src.x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.36f);

        var loc = sys.Warp(sender.Loc, true);
        sender.Position = loc.ToPosition();
        sender.ChangeDir(Dir.S);
        sender.UpdateLoc(loc);
        if (sender is Player) { // TODO: player のメソッド内で処理する
            ((Player)sender).SyncCameraPosition();
        }
        yield return new WaitForSeconds(0.4f);
    }

    public override IEnumerator Hit(CharacterBase sender, CharacterBase target, MainSystem sys) {
        yield return Use(target, sys);
    }

}
