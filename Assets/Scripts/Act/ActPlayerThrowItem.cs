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
    }

    protected override IEnumerator RunAnimation(MainSystem sys) {
        var fitem = FieldItemFactory.CreateFromItem(_item, _player.Loc);
        fitem.BringToFront();

        Vector3 src = _player.Loc.ToPosition();
        Vector3 dst = _targetLoc.ToPosition();
        float speed = Config.ThrowItemSpeed;
        float duration = Vector3.Distance(src, dst) / speed;

        float elapsed = 0;
        while (elapsed <= duration) {
            elapsed += Time.deltaTime;

            float x = Mathf.Lerp(src.x, dst.x, elapsed / duration);
            float y = Mathf.Lerp(src.y, dst.y, elapsed / duration);
            fitem.UpdatePosition(new Vector3(x, y, 0));
            yield return null;
        }

        fitem.ResetZOrder();

        if (_target == null) { // 床に落とす
            sys.FallItemToFloor(_fallLoc, fitem);
        }
        else { // ターゲットにヒット
            // TODO:ヒット判定

        }
    }

    public override void Apply(MainSystem sys) {
        _player.RemoveItem(_item);



    }
}
