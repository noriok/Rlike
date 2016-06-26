using UnityEngine;

public class FieldItem {
    public Item Item { get; private set; }
    public Loc Loc { get; private set; }
    public Vector3 Position {
        get { return _gobj.transform.position; }
        set { _gobj.transform.position = value; }
    }
    public bool Visible {
        get { return _visible; }
        set {
            if (_visible != value) {
                _gobj.SetActive(value);
                _visible = value;
            }
        }
    }

    private GameObject _gobj;
    private bool _discovered = false; // 視認済み
    private bool _visible = false;

    public FieldItem(Item item, Loc loc, GameObject gobj) {
        Item = item;
        Loc = loc;
        _gobj = gobj;
        _gobj.transform.position = loc.ToPosition();
        _gobj.SetActive(_visible);
    }

    public void Destroy() {
        GameObject.Destroy(_gobj);
    }

    public void UpdateLoc(Loc loc) {
        Loc = loc;
        Position = loc.ToPosition();
    }

    private void ChangeSortingLayerName(string sortingLayerName) {
        var renderer = _gobj.GetComponent<SpriteRenderer>();
        renderer.sortingLayerName = sortingLayerName;
    }

    public void BringToFront() {
        ChangeSortingLayerName("Front Base");
    }

    public void ResetZOrder() {
        ChangeSortingLayerName("Item Base");
    }

    // プレイヤーに視認された
    public void OnDiscovered(bool isPlayerBlind) {
        _discovered = true;
        if (!isPlayerBlind) {
            Visible = true;
        }
    }

    // プレイヤーが目つぶし状態になった/解除された
    public void OnPlayerBlindStatusChanged(bool isBlind) {
        if (isBlind) {
            Visible = false;
        }
        else {
            Visible = _discovered;
        }
    }

    public void ResetVisible() {
        Visible = _discovered;
    }
}

