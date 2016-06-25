using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

public class Player : CharacterBase {
    public List<Item> Items { get { return _items; } }

    private List<Item> _items = new List<Item>();
    private Dictionary<Dir, GameObject> _dirs = new Dictionary<Dir, GameObject>();

    private Camera _camera;
    private GameObject _minimapLayer;

    public Player(Loc loc, GameObject gobj) : base(loc, gobj) {
        Hp = MaxHp = 255;
        var textHp = GameObject.Find("Canvas/Header/Text_HP_Value").GetComponent<Text>();
        textHp.text = string.Format("{0}/{1}", Hp, MaxHp);

        var pos = gobj.transform.position;
        float d = Config.ChipSize / 2;
        var n  = Utils.Instantiate("Prefabs/Dir/dir-N", new Vector3(pos.x, pos.y + d + d/3, pos.z));
        var ne = Utils.Instantiate("Prefabs/Dir/dir-NE", new Vector3(pos.x + d, pos.y + d, pos.z));
        var e  = Utils.Instantiate("Prefabs/Dir/dir-E", new Vector3(pos.x + d, pos.y, pos.z));
        var se = Utils.Instantiate("Prefabs/Dir/dir-SE", new Vector3(pos.x + d, pos.y - d, pos.z));
        var s  = Utils.Instantiate("Prefabs/Dir/dir-S", new Vector3(pos.x, pos.y - d - d/3, pos.z));
        var sw = Utils.Instantiate("Prefabs/Dir/dir-SW", new Vector3(pos.x - d, pos.y - d, pos.z));
        var w  = Utils.Instantiate("Prefabs/Dir/dir-W", new Vector3(pos.x - d, pos.y, pos.z));
        var nw = Utils.Instantiate("Prefabs/Dir/dir-NW", new Vector3(pos.x - d, pos.y + d, pos.z));

        _dirs.Add(Dir.N, n);
        _dirs.Add(Dir.NE, ne);
        _dirs.Add(Dir.E, e);
        _dirs.Add(Dir.SE, se);
        _dirs.Add(Dir.S, s);
        _dirs.Add(Dir.SW, sw);
        _dirs.Add(Dir.W, w);
        _dirs.Add(Dir.NW, nw);
        foreach (var kv in _dirs) {
            kv.Value.transform.SetParent(gobj.transform);
            kv.Value.SetActive(false);
        }
    }

    public override void UpdateLoc(Loc loc) {
        base.UpdateLoc(loc);
        SyncCameraPosition();
    }

    public void SyncCameraPosition() {
        if (_camera == null) {
            _camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        }
        if (_minimapLayer == null) {
            _minimapLayer = GameObject.Find(LayerName.Minimap);
        }

        float cameraZ = _camera.transform.position.z;
        float x = Position.x;
        float y = Position.y;
        _camera.transform.position = new Vector3(x, y + Config.CameraOffsetY, cameraZ);
        _minimapLayer.transform.localPosition = new Vector3(x + Config.MinimapOffsetX, y + Config.MinimapOffsetY, 0);
    }

    public override void ShowDirection(Dir dir) {
        HideDirection();
        _dirs[dir].SetActive(true);
    }

    public override void HideDirection() {
        foreach (var kv in _dirs) {
            kv.Value.SetActive(false);
        }
    }

    public void ShowDirectionAll() {
        foreach (var kv in _dirs) {
            kv.Value.SetActive(true);
        }
    }

    // TODO:作り直し
    public void UpdateHp(int hp) {
        hp = Utils.Clamp(hp, 0, MaxHp);
        Hp = hp;

        var imageFg = GameObject.Find("Canvas/Header/Image_HP_FG").GetComponent<Image>();
        var textHp = GameObject.Find("Canvas/Header/Text_HP_Value").GetComponent<Text>();
        textHp.text = string.Format("{0}/{1}", hp, MaxHp);
        imageFg.fillAmount = hp / MaxHp;
    }

    public override IEnumerator HealAnim(int delta) {
        float fromHp = Hp;
        float toHp = Utils.Clamp(Hp + delta, 0, MaxHp);

        var fromAmount = fromHp / MaxHp;
        var toAmount = toHp / MaxHp;
        Debug.LogFormat("amount fm:{0} to:{1}", fromAmount, toAmount);

        var imageFg = GameObject.Find("Canvas/Header/Image_HP_FG").GetComponent<Image>();
        var textHp = GameObject.Find("Canvas/Header/Text_HP_Value").GetComponent<Text>();
        textHp.text = string.Format("{0}/{1}", toHp, MaxHp);

        imageFg.fillAmount = toAmount;
        float duration = 0.3f;
        float elapsed = 0;
        while (elapsed <= duration) {
            elapsed += Time.deltaTime;
            float p = UTween.Ease(EaseType.OutQuad, fromAmount, toAmount, elapsed / duration);
            imageFg.fillAmount = p;
            yield return null;
        }
        imageFg.fillAmount = toAmount;
    }

    public override IEnumerator DamageAnim(int delta) {
        float fromHp = Hp;
        float toHp = Utils.Clamp(Hp - delta, 0, MaxHp);

        var fromAmount = fromHp / MaxHp;
        var toAmount = toHp / MaxHp;
        Debug.LogFormat("amount fm:{0} to:{1}", fromAmount, toAmount);

        var imageFg = GameObject.Find("Canvas/Header/Image_HP_FG").GetComponent<Image>();
        var imageMg = GameObject.Find("Canvas/Header/Image_HP_MG").GetComponent<Image>();

        var textHp = GameObject.Find("Canvas/Header/Text_HP_Value").GetComponent<Text>();
        textHp.text = string.Format("{0}/{1}", toHp, MaxHp);

        imageFg.fillAmount = toAmount;
        imageMg.fillAmount = fromAmount;
        yield return new WaitForSeconds(0.53f);

        float duration = 0.3f;
        float elapsed = 0;
        while (elapsed <= duration) {
            elapsed += Time.deltaTime;
            float p = UTween.Ease(EaseType.OutQuad, fromAmount, toAmount, elapsed / duration);
            imageMg.fillAmount = p;
            yield return null;
        }
        imageMg.fillAmount = toAmount;
    }

    public override string ToString() {
        return string.Format("P: Loc:{0}", Loc);
    }

    public override void OnTurnStart() {
        ActCount = 1;
    }

    private int _counter = 0;
    public override void OnTurnEnd() {
        base.OnTurnEnd();

        if (++_counter % 7 == 0) {
            // HP 自然回復
            if (Hp > 0) {
                int heal = 1;
                float toHp = Utils.Clamp(Hp + heal, 0, MaxHp);

                var imageFg = GameObject.Find("Canvas/Header/Image_HP_FG").GetComponent<Image>();
                var textHp = GameObject.Find("Canvas/Header/Text_HP_Value").GetComponent<Text>();
                textHp.text = string.Format("{0}/{1}", toHp, MaxHp);
                imageFg.fillAmount = toHp / MaxHp;
                HealHp(heal);
            }
        }
    }

    public void AddItem(Item item) {
        // // 石はまとまる
        // if (item.Type == ItemType.Stone) {
        //     for (int i = 0; i < _items.Count; i++) {
        //         if (_items[i].Type == ItemType.Stone) {
        //             _items[i].Inc(item.Count);
        //             return;
        //         }
        //     }
        // }
        _items.Add(item);
    }

    public Item RemoveItem(Item item) {
        for (int i = 0; i < _items.Count; i++) {
            if (object.ReferenceEquals(_items[i], item)) {
                Assert.IsTrue(item.Count > 0);

                _items.RemoveAt(i);
                return item;
            }
        }

        Assert.IsTrue(false);
        return null;
    }

    // TODO:UpdateLoc に揃えて UpdateDir にする
    public override void ChangeDir(Dir dir) {
        // if (_dir == dir) return; // TODO:斜めの画像がある場合は dir で判定する
        _dir = dir;

        string name = "ToN";
        switch (dir) {
        case Dir.N:  name = "ToN"; break;
        case Dir.NE: name = "ToNE"; break;
        case Dir.E:  name = "ToE"; break;
        case Dir.SE: name = "ToSE"; break;
        case Dir.S:  name = "ToS"; break;
        case Dir.SW: name = "ToSW"; break;
        case Dir.W:  name = "ToW"; break;
        case Dir.NW: name = "ToNW"; break;
        }

        if (_triggerName == name) return;
        _triggerName = name;

        var anim = _gobj.GetComponent<Animator>();
        anim.SetTrigger(name);
    }

    public override void OnStatusAdded(StatusType status) {
        if (status == StatusType.Invisible) {
            _gobj.SetAlpha(0.4f);
        }
    }

    public override void OnStatusRemoved(StatusType status) {
        if (status == StatusType.Invisible) {
            _gobj.SetAlpha(1.0f);
        }
    }

    public void ClearItems() {
        _items.Clear();
    }
}
