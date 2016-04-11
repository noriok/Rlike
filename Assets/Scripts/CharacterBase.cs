using UnityEngine;
// using System.Collections;

public class CharacterBase {
    public int Row { get { return _loc.Row; } }
    public int Col { get { return _loc.Col; } }
    public Loc Loc { get { return _loc; } }

    public Vector3 Position {
        get { return _gobj.transform.position; }
        set { _gobj.transform.position = value; }
    }

    public int ActCount { get; set; }

    private Loc _loc;
    private GameObject _gobj;

    public CharacterBase(int row, int col, GameObject gobj) {
        ActCount = 1;
        _loc = new Loc(row, col);
        _gobj = gobj;
        float x =  Config.ChipSize * col;
        float y = -Config.ChipSize * row;
        _gobj.transform.position = new Vector3(x, y, 0);
    }

    public void UpdateLoc(Loc loc) {
        _loc = loc;
    }

    public void ChangeDir(Dir dir) {
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

        var anim = _gobj.GetComponent<Animator>();
        anim.SetTrigger(name);
        DLog.D("name: {0}", name);
    }
}
