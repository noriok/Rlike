using System;
using UnityEngine;
// using System.Collections;

public class CharacterBase {
    public int Row { get { return _loc.Row; } }
    public int Col { get { return _loc.Col; } }
    public Loc Loc { get { return _loc; } }
    public Dir Dir { get { return _dir; } }

    public int Hp { get; private set; }
    public int MaxHp { get; private set; }

    public Vector3 Position {
        get { return _gobj.transform.position; }
        set { _gobj.transform.position = value; }
    }

    public int ActCount { get; set; }

    private Loc _loc;
    private Dir _dir = Dir.S;
    // 同じトリガーを繰り返し実行しないように前回のトリガーを記憶する
    // 斜め画像を用意するまでの暫定対応
    private string _triggerName = "ToS";
    private GameObject _gobj;

    public CharacterBase(int row, int col, GameObject gobj) {
        ActCount = 1;
        _loc = new Loc(row, col);
        _gobj = gobj;
        float x =  Config.ChipSize * col;
        float y = -Config.ChipSize * row;
        _gobj.transform.position = new Vector3(x, y, 0);

        Hp = MaxHp = 100;
    }

    public void Destroy() {
        GameObject.Destroy(_gobj);
    }

    public Loc Front() {
        return Loc.Forward(Dir);
    }

    public virtual void DamageHp(int damage) {
        Hp = Utils.Clamp(Hp - damage, 0, 100);
    }

    public void UpdateLoc(Loc loc) {
        _loc = loc;
    }

    public void PlayAnimation() {
        var anim = _gobj.GetComponent<Animator>();
        anim.enabled = true;
    }

    public void StopAnimation() {
        var anim = _gobj.GetComponent<Animator>();
        anim.enabled = false;
    }

    // TODO:UpdateLoc に揃えて UpdateDir にする
    public void ChangeDir(Dir dir) {
        // if (_dir == dir) return; // TODO:斜めの画像がある場合は dir で判定する
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
}
