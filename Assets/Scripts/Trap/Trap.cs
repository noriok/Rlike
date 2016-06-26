using UnityEngine;

public abstract class Trap : SimpleFieldObject {
    private bool _discovered = false;

    public Trap(Loc loc, GameObject gobj) : base(loc, gobj) {
        Visible = false;
    }

    public override bool IsObstacle() {
        return false;
    }

    public abstract string Name();
    public abstract string Description();
    public abstract string ImagePath();

    // プレイヤーに発見された TODO: FieldItem と同じ
    public void OnDiscovered(bool isPlayerBlind) {
        _discovered = true;
        if (!isPlayerBlind) {
            Visible = true;
        }
    }

    // プレイヤーが目つぶし状態になった/解除された TODO: FieldItem と同じ
    public void OnPlayerBlindStatusChanged(bool isBlind) {
        if (isBlind) {
            Visible = false;
        }
        else {
            Visible = _discovered;
        }
    }
}
