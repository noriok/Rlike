using UnityEngine;
// using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Map {
    private const string TestMap = @"
#######################
#.....................#
#.....................#
#.....................#
#.....................#
#####.#################
    #.#
    #.#    ##############
    #.#    #............#
    #.######............#
    #...................#
    ########............#
           #............#
           ##############
";
    private char[,] _map;

    public int Rows { get { return _map.GetLength(0); } }
    public int Cols { get { return _map.GetLength(1); } }

    private GameObject _mapLayer;

    private List<FieldObject> _fieldObjects = new List<FieldObject>();

    public Map() {
        var lines = TestMap.Trim().Split(new[] { '\n' });

        int rows = lines.Length;
        int cols = lines.Select(s => s.Length).Max();

        _map = new char[rows, cols];
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                _map[i, j] = MapChar.Floor;
            }
            for (int j = 0; j < lines[i].Length; j++) {
                _map[i, j] = lines[i][j];
            }
        }

        _mapLayer = new GameObject("MapLayer");

        var flat = Resources.Load("Prefabs/MapChip/pipo-map001_0");
        var mountain = Resources.Load("Prefabs/MapChip/pipo-map001_at-yama2_0");
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                float x = j * Config.ChipSize;
                float y = i * Config.ChipSize;

                var pos = new Vector3(x, -y, 0);
                var floatObj = (GameObject)GameObject.Instantiate(flat, pos, Quaternion.identity);
                floatObj.transform.SetParent(_mapLayer.transform);

                switch (_map[i, j]) {
                case '#':
                    var mtObj = (GameObject)GameObject.Instantiate(mountain, pos, Quaternion.identity);
                    mtObj.transform.SetParent(_mapLayer.transform);
                    break;
                }
            }
        }

        // マップ上のオブジェクト
        _fieldObjects.Add(FieldObjectFactory.CreateBonfire(new Loc(3, 3), _mapLayer));
        _fieldObjects.Add(FieldObjectFactory.CreateTreasure(new Loc(4, 4), _mapLayer));
        _fieldObjects.Add(FieldObjectFactory.CreateNoticeBoard(new Loc(1, 2), _mapLayer, "立て札のメッセージ"));


        // ワナ
        _fieldObjects.Add(FieldObjectFactory.CreateTrapHeal(new Loc(3, 5), _mapLayer));
        _fieldObjects.Add(FieldObjectFactory.CreateTrapWarp(new Loc(3, 6), _mapLayer));
        _fieldObjects.Add(FieldObjectFactory.CreateTrapDamage(new Loc(3, 7), _mapLayer));
        _fieldObjects.Add(FieldObjectFactory.CreateTrapSummon(new Loc(3, 8), _mapLayer));
    }

    private T FindFieldObject<T>(Loc loc) where T : FieldObject {
        foreach (var obj in _fieldObjects) {
            if (obj.Loc == loc && obj is T) {
                return (T)obj;
            }
        }
        return null;
    }

    public Treasure FindTreasure(Loc loc) {
        return FindFieldObject<Treasure>(loc);
    }

    public NoticeBoard FindNoticeBoard(Loc loc) {
        return FindFieldObject<NoticeBoard>(loc);
    }

    public Trap FindTrap(Loc loc) {
        return FindFieldObject<Trap>(loc);
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
        return _map[row, col] == MapChar.Wall;
    }

    public bool IsFloor(Loc loc) {
        return IsFloor(loc.Row, loc.Col);
    }

    public bool IsFloor(int row, int col) {
        if (OutOfMap(row, col)) return false;
        return _map[row, col] == MapChar.Floor;
    }

    // FieldObject かつ Obstacle なオブジェクト
    public bool ExistsObstacleFieldObject(Loc loc) {
        foreach (var obj in _fieldObjects) {
            if (loc == obj.Loc && obj.IsObstacle()) {
                return true;
            }
        }
        return false;
    }

    // fm から dir に向かって前進できるか
    public bool CanAdvance(Loc fm, Dir dir) {
        Loc to = fm.Forward(dir);
        if (!(IsFloor(fm) && IsFloor(to))) return false;

        // 障害物が配置されているか
        if (ExistsObstacleFieldObject(to)) return false;

        if (fm.Row != to.Row && fm.Col != to.Col) { // 斜め移動


            if (IsFloor(fm.Row, to.Col) && IsFloor(to.Row, fm.Col)) {
                return true;
            }
            return false;
        }

        return true;
    }
}
