using UnityEngine;
using System.Collections;

// アイテムを投げる。ただし、石は別処理(ActPlayerShoot)
public class ActPlayerThrowItem : Act { // TODO : ActPlayer
    private Player _player;
    private Item _item;
    private Loc _targetLoc; // アイテムを移動させる先(アニメ)
    private Loc _fallLoc; // アイテムが落ちる場所
    private CharacterBase _target;

    public ActPlayerThrowItem(Player player, Item item, Loc targetLoc, Loc fallLoc, CharacterBase target) : base(player) {
        _player = player;
        _item = item;
        _targetLoc = targetLoc;
        _fallLoc = fallLoc;
        _target = target;

        Debug.Log("player.Loc = " + player.Loc);
        Debug.Log("targetLoc = " + targetLoc);
        Debug.Log("fallLoc = " + fallLoc);
        Debug.Log("target = " + target);
    }

    protected override IEnumerator Run(MainSystem sys) {
        var fitem = FieldItemFactory.CreateFromItem(_item, _player.Loc);
        fitem.BringToFront();

        Vector3 src = _player.Loc.ToPosition();
        Vector3 dst = _targetLoc.ToPosition();
        float speed = _player.Dir.IsDiagonal() ? Config.ItemThrowDiagonalSpeed : Config.ItemThrowSpeed;
        float duration = Vector3.Distance(src, dst) / speed;
        yield return CAction.Lerp(duration, src, dst, pos => {
            fitem.Position = pos;
        });

        fitem.ResetZOrder();

        if (_target == null) { // 床に落とす
            sys.FallItemToFloor(_fallLoc, fitem);
        }
        else { // ターゲットにヒット
            // TODO:ヒット判定
            fitem.Destroy();
            yield return fitem.Item.Hit(_player, _target, sys);
        }
    }

    public override void OnFinished(MainSystem sys) {
        _player.RemoveItem(_item);
    }
}
