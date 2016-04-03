﻿using UnityEngine;
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
        _gobj.transform.position = new Vector3(col, -row, 0);
        // Debug.Log("ctr CharacterBase");
    }

    public void UpdateLoc(Loc loc) {
        _loc = loc;
    }
}
