using UnityEngine;
using System.Collections;

public class CharacterBase {
    public int Row { get { return _loc.Row; } }
    public int Col { get { return _loc.Col; } }

    public Vector3 Position {
        get { return _gobj.transform.position; }
        set { _gobj.transform.position = value; }
    }

    private Loc _loc;
    private GameObject _gobj;

    public CharacterBase(int row, int col, GameObject gobj) {
        _loc = new Loc(row, col);
        _gobj = gobj;
        Debug.Log("ctr CharacterBase");
    }
}
