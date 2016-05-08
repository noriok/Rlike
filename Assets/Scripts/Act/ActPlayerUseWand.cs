using UnityEngine;
using System.Collections;

public class ActPlayerUseWand : Act {
    private Player _player;
    private Item _item;

    public ActPlayerUseWand(Player player, Item item) : base(player) {
        _player = player;
        _item = item;
    }

    protected override IEnumerator RunAnimation(MainSystem sys) {
        CharacterBase hitTarget;
        Loc to = sys.FindHitTarget(_player.Loc, _player.Dir, out hitTarget);

        // 魔法弾を飛ばす
        var obj = Resources.Load("Prefabs/Effect/magic-ball");
        var gobj = (GameObject)GameObject.Instantiate(obj);
        yield return CAction.Move(gobj, _player.Loc, to);

        GameObject.Destroy(gobj);

        if (hitTarget != null) {
            yield return _item.Hit(_player, hitTarget, sys);
        }
    }

    public override void Apply(MainSystem sys) {

    }

}
