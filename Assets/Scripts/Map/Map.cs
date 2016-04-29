using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;

public class Map {
    private char[,] _mapData;

    public int Rows { get { return _mapData.GetLength(0); } }
    public int Cols { get { return _mapData.GetLength(1); } }

    private GameObject _mapLayer;

    public Map(char[,] mapData) {
        _mapData = mapData;
        int rows = _mapData.GetLength(0);
        int cols = _mapData.GetLength(1);

        _mapLayer = new GameObject(LayerName.Map);

        var flat = Resources.Load("Prefabs/MapChip/pipo-map001_0");
        var mountain = Resources.Load("Prefabs/MapChip/pipo-map001_at-yama2_0");
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                var pos = new Loc(i, j).ToPosition();
                var floatObj = (GameObject)GameObject.Instantiate(flat, pos, Quaternion.identity);
                floatObj.transform.SetParent(_mapLayer.transform);

                switch (_mapData[i, j]) {
                case MapChar.Wall:
                    var mtObj = (GameObject)GameObject.Instantiate(mountain, pos, Quaternion.identity);
                    mtObj.transform.SetParent(_mapLayer.transform);
                    break;
                }
            }
        }
    }

    private GameObject Create(Loc loc, string path, GameObject layer) {
        var obj = Resources.Load(path);
        var gobj = (GameObject)GameObject.Instantiate(obj, loc.ToPosition(), Quaternion.identity);
        gobj.transform.SetParent(layer.transform);
        return gobj;
    }

    public bool OutOfMap(Loc loc) {
        return OutOfMap(loc.Row, loc.Col);
    }

    public bool OutOfMap(int row, int col) {
        return row < 0 || row >= Rows || col < 0 || col >= Cols;
    }

    public bool IsWall(Loc loc) {
        return IsWall(loc.Row, loc.Col);
    }

    public bool IsWall(int row, int col) {
        if (OutOfMap(row, col)) return true; // マップ外は壁扱い
        return _mapData[row, col] == MapChar.Wall;
    }

    public bool IsFloor(Loc loc) {
        return IsFloor(loc.Row, loc.Col);
    }

    public bool IsFloor(int row, int col) {
        if (OutOfMap(row, col)) return false;
        return _mapData[row, col] == MapChar.Room || _mapData[row, col] == MapChar.Passage;
    }
}
