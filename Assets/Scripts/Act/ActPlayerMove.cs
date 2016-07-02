using UnityEngine;
// using System.Collections;

public class ActPlayerMove : Act {
    private Dir _dir;

    private float _elapsed;
    private Vector3 _srcPos;
    private Vector3 _dstPos;
    private Loc _nextLoc;

    private bool _isFirst = true;
    private Player _player;
    private FieldItem _fieldItem; // 移動先に落ちているアイテム

    public ActPlayerMove(Player player, Dir dir, FieldItem fieldItem) : base(player) {
        _dir = dir;
        _player = player;
        _nextLoc = player.Loc.Forward(dir);
        _fieldItem = fieldItem;

        _elapsed = 0;
        _srcPos = player.Loc.ToPosition();
        _dstPos = player.Loc.Forward(dir).ToPosition();
    }

    public override bool IsManualUpdate() {
        return true;
    }

    public override bool IsMoveAct() {
        return true;
    }

   public override void Update(MainSystem sys) {
        if (_isFirst) {
            _isFirst = false;
            Actor.UpdateDir(_dir);
            sys.UpdateSpot(_nextLoc);

            Room prev = sys.FindRoom(_player.Loc);
            Room next = sys.FindRoom(_nextLoc);
            if (prev != null && next != null) { // 部屋内を移動した
                sys.OnRoomMoving(next, _nextLoc);
            }
            else if (prev == null && next != null) { // 部屋に入った
                sys.OnRoomEntering(next, _nextLoc);
            }
            else if (prev != null && next == null) { // 部屋から通路に出た
                sys.OnRoomExiting(prev, _nextLoc);
            }
            else {
                sys.OnPassageMoving(_nextLoc); // 通路を移動した
            }
        }

        _elapsed += Time.deltaTime;
        float t = _elapsed / Config.WalkDuration;
        float x = Mathf.Lerp(_srcPos.x, _dstPos.x, t);
        float y = Mathf.Lerp(_srcPos.y, _dstPos.y, t);
        Actor.Position = new Vector3(x, y, 0);

        _player.SyncCameraPosition();
        sys.UpdatePassageSpotlightPosition(Actor.Position);

        if (_elapsed >= Config.WalkDuration) {
            _animationFinished = true;
            // 位置ずれ防止
            Actor.Position = _dstPos;
            _player.SyncCameraPosition();
        }
    }

    public override void OnFinished(MainSystem sys) {
        Loc prevLoc = _player.Loc;
        Actor.UpdateLoc(_nextLoc);

        Room prev = sys.FindRoom(prevLoc);
        Room next = sys.FindRoom(_nextLoc);
        Debug.LogFormat("--> {0} -> {1}", prevLoc, _nextLoc);

        if (prev != null && next != null) { // 部屋内の移動
            sys.OnRoomMoved(next, _nextLoc);
        }
        if (prev == null && next != null) { // 部屋に入った
            sys.OnRoomEntered(next, _nextLoc);
        }
        else if (prev != null && next == null) { // 部屋から通路に出た
            sys.OnRoomExited(prev, _nextLoc);
        }
        else {
            sys.OnPassageMoved(_nextLoc); // 通路を移動した
        }

        if (_fieldItem != null) {
            Item item = _fieldItem.Item;
            sys.Msg_TakeItem(item);
            if (item.Type == ItemType.Gold) {
                sys.IncGold(100);
            }
            else {
                // TODO:持ち物がいっぱいなら拾えない
                _player.AddItem(item);
            }
            sys.RemoveFieldItem(_fieldItem);
        }

        sys.UpdateMinimap();
    }
}
