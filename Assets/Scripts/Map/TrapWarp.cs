using UnityEngine;
using System.Collections;

public class TrapWarp : Trap {
    public TrapWarp(Loc loc, GameObject gobj) : base(loc, gobj) {

    }

    public override IEnumerator RunAnimation(CharacterBase sender, MainSystem sys) {
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

        // TODO:ワープ後の位置
        var loc = sys.RandomRoomLoc(sender.Loc);
        sender.Position = loc.ToPosition();
        sender.ChangeDir(Dir.S);
        sender.UpdateLoc(loc);
        if (sender is Player) {
            ((Player)sender).SyncCameraPosition();
        }
        yield return new WaitForSeconds(0.4f);
    }

}
