using UnityEngine;
// using System.Collections;

public class ActPlayerMove : Act {
    private Dir _dir;

    private float _elapsed;
    private Vector3 _srcPos;
    private Vector3 _dstPos;

    private bool _isFirst = true;
    private Player _player;
    private FieldItem _fieldItem; // 移動先に落ちているアイテム

    public ActPlayerMove(Player player, Dir dir, FieldItem fieldItem) : base(player) {
        _dir = dir;
        _player = player;
        _fieldItem = fieldItem;

        _elapsed = 0;
        _srcPos = player.Loc.ToPosition();
        _dstPos = player.Loc.Forward(dir).ToPosition();
    }

    public override bool IsMoveAct() {
        return true;
    }

    // protected override IEnumerator RunAnimation(MainSystem sys) {
    //     Actor.ChangeDir(Utils.ToDir(_drow, _dcol));

    //     // TODO: 一歩一歩が連続していないので矢印が点滅してしまう
    //     //Actor.ShowDirection(Utils.ToDir(_drow, _dcol));

    //     yield return CAction.Walk(Actor, _drow, _dcol, (x, y) => {
    //         _player.SyncCameraPosition();
    //     });

    //     // Actor.HideDirection();
    // }

    public override void Apply(MainSystem sys) {
        var nextLoc = Actor.Loc.Forward(_dir);
        DLog.D("{0} move {1} -> {2}", Actor, Actor.Loc, nextLoc);
        Actor.UpdateLoc(nextLoc);

        if (_fieldItem != null) {
            Debug.Log("アイテムを拾った");

            Item item = _fieldItem.Item;
            if (item.Type == ItemType.Gold) {
                Debug.LogFormat("{0} G 手に入れた", 100);
                sys.IncGold(100);
                sys.RemoveFieldItem(_fieldItem);
            }
            else {
                // TODO:持ち物がいっぱいなら拾えない
                _player.AddItem(item);
                sys.RemoveFieldItem(_fieldItem);
            }
        }
    }

    public override bool IsManualUpdate() {
        return true;
    }

    public override void Update(MainSystem sys) {
        if (_isFirst) {
            Actor.ChangeDir(_dir);
            _isFirst = false;
        }

        _elapsed += Time.deltaTime;
        float t = _elapsed / Config.WalkDuration;
        float x = Mathf.Lerp(_srcPos.x, _dstPos.x, t);
        float y = Mathf.Lerp(_srcPos.y, _dstPos.y, t);
        Actor.Position = new Vector3(x, y, 0);

        _player.SyncCameraPosition();
        if (_elapsed >= Config.WalkDuration) {
            _animationFinished = true;
            // 位置ずれ防止
            Actor.Position = _dstPos;
            _player.SyncCameraPosition();
        }
    }
}
