// using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatusOne {
    public GameObject obj;
    public int turn;

    public StatusOne(GameObject obj, int turn) {
        this.obj = obj;
        this.turn = turn;
    }
}

public abstract class CharacterBase {
    public int Row { get { return _loc.Row; } }
    public int Col { get { return _loc.Col; } }
    public Loc Loc { get { return _loc; } }
    public Dir Dir { get { return _dir; } }
    public bool Visible {
        get { return _visible; }
        set {
            if (_visible != value) {
                _gobj.SetActive(value);
                _visible = value;
            }
        }
    }
    private bool _visible;

    public int Hp { get; protected set; }
    public int MaxHp { get; protected set; }

    public Vector3 Position {
        get { return _gobj.transform.position; }
        set { _gobj.transform.position = value; }
    }

    public int ActCount { get; set; }

    private Loc _loc;
    protected Dir _dir = Dir.S;
    // 同じトリガーを繰り返し実行しないように前回のトリガーを記憶する
    // 斜め画像を用意するまでの暫定対応
    protected string _triggerName = "ToS";
    protected GameObject _gobj;

    // TODO:アニメーションの一時停止処理
    private float _animationSpeed;

//    private Dictionary<Status, GameObject> _status = new Dictionary<Status, GameObject>();
    private Dictionary<StatusType, StatusOne> _status = new Dictionary<StatusType, StatusOne>();

    public CharacterBase(Loc loc, GameObject gobj) {
        ActCount = 0;
        _loc = loc;
        _gobj = gobj;
        _gobj.transform.position = loc.ToPosition();
        _visible = true;
        _animationSpeed = _gobj.GetComponent<Animator>().speed;

        Hp = MaxHp = 40;
    }

    public void Destroy() {
        GameObject.Destroy(_gobj);
    }

    public Loc Front() {
        return Loc.Forward(Dir);
    }

    public void HealHp(int heal) {
        Hp = Utils.Clamp(Hp + heal, 0, MaxHp);
    }

    public virtual void DamageHp(int damage) {
        Hp = Utils.Clamp(Hp - damage, 0, MaxHp);

        RemoveStatus(StatusType.Sleep);
        RemoveStatus(StatusType.Freeze);
    }

    public virtual void UpdateLoc(Loc loc) {
        _loc = loc;
        Position = loc.ToPosition();
    }

    public void AddStatus(StatusType status) {
        AddStatus(status, int.MaxValue);
    }

    private GameObject CreateStatusSymbol(string pathName) {
        var st = (GameObject)GameObject.Instantiate(Resources.Load(pathName), Vector3.zero, Quaternion.identity);
        st.transform.SetParent(_gobj.transform);
        st.transform.localPosition = new Vector3(0, 0.18f, 0);
        return st;
    }

    // 内部の状態管理。
    // TODO: 見た目の変化は、OnStatusAdded, OnStatusRemoved で行う
    public void AddStatus(StatusType status, int depth) {
        if (_status.ContainsKey(status)) return;

        if (status == StatusType.Sleep) {
            var st = CreateStatusSymbol("Prefabs/Animations/status-sleep");
            _status.Add(status, new StatusOne(st, depth));
            StopAnimation();
        }
        else if (status == StatusType.Freeze) {
            var st = CreateStatusSymbol("Prefabs/Animations/status-freeze");
            _status.Add(status, new StatusOne(st, depth));
            StopAnimation();
        }
        else if (status == StatusType.Invisible) {
            // 状態異常マークはないので null を格納
            _status.Add(status, new StatusOne(null, depth));
        }

        OnStatusAdded(status);
    }

    public void RemoveStatus(StatusType status) {
        if (_status.ContainsKey(status)) {
            GameObject.Destroy(_status[status].obj);
            _status.Remove(status);

            if (status == StatusType.Sleep) {
                PlayAnimation();
            }
            else if (status == StatusType.Freeze) {
                PlayAnimation();
            }

            OnStatusRemoved(status);
        }
    }

    public void RemoveAllStatus() {
        foreach (var kv in _status) {
            GameObject.Destroy(kv.Value.obj);
        }
        _status.Clear();
    }

    public void PlayAnimation() {
        var anim = _gobj.GetComponent<Animator>();
        anim.speed = _animationSpeed;
    }

    public void StopAnimation() {
        var anim = _gobj.GetComponent<Animator>();
        anim.speed = 0;
    }

    // TODO:UpdateLoc に揃えて UpdateDir にする
    public virtual void ChangeDir(Dir dir) {
        // if (_dir == dir) return; // TODO:斜めの画像がある場合は dir で判定する
        // TODO:敵が消えている場合の処理。プレイヤー追跡ロジックの動作がおかしい。
        if (IsInvisible()) return;
        if (!Visible) return;

        _dir = dir;

        string name = "ToN";
        switch (dir) {
        case Dir.N:  name = "ToN"; break;
        case Dir.NE: name = "ToN"; break;
        case Dir.E:  name = "ToE"; break;
        case Dir.SE: name = "ToS"; break;
        case Dir.S:  name = "ToS"; break;
        case Dir.SW: name = "ToS"; break;
        case Dir.W:  name = "ToW"; break;
        case Dir.NW: name = "ToN"; break;
        }

        if (_triggerName == name) return;
        _triggerName = name;

        var anim = _gobj.GetComponent<Animator>();
        anim.SetTrigger(name);
    }

    public virtual IEnumerator HealAnim(int delta) {
        yield break;
    }

    public virtual IEnumerator DamageAnim(int delta) {
        yield break;
    }

    public bool IsSleep() {
        return _status.ContainsKey(StatusType.Sleep);
    }

    public bool IsFreeze() {
        return _status.ContainsKey(StatusType.Freeze);
    }

    public bool IsBlind() {
        return _status.ContainsKey(StatusType.Blind);
    }

    public bool IsInvisible() {
        return _status.ContainsKey(StatusType.Invisible);
    }

    public IEnumerator FadeIn() {
        return CAction.Fade(_gobj, 0, 1f, 0.5f);
    }

    public virtual void ShowDirection(Dir dir) {
    }

    public virtual void HideDirection() {
    }

    public abstract void OnTurnStart();

    public virtual void OnTurnEnd() {
        foreach (var kv in _status) {
            Debug.LogFormat("status:{0} turn:{1}", kv.Key, kv.Value.turn);
            if (--kv.Value.turn <= 0) {
                RemoveStatus(kv.Key);
                break; // TODO:status を削除する方法
            }
        }
    }

    public virtual void OnStatusAdded(StatusType status) {
    }

    public virtual void OnStatusRemoved(StatusType status) {
    }
}
