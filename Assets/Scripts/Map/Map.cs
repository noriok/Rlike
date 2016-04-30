using UnityEngine;
using System;
// using System.Collections;
using System.Collections.Generic;
// using System.Linq;

public class Map {
    private const int NoRoom = -1;
    private char[,] _mapData;

    public int Rows { get { return _mapData.GetLength(0); } }
    public int Cols { get { return _mapData.GetLength(1); } }

    private GameObject _mapLayer;

    private Room[] _rooms;
    private int[,] _roomMap;

    public Map(char[,] mapData) {
        _mapData = mapData;
        int rows = _mapData.GetLength(0);
        int cols = _mapData.GetLength(1);

        _rooms = GetRooms();
        _roomMap = new int[rows, cols];
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                _roomMap[i, j] = NoRoom;
            }
        }
        for (int i = 0; i < _rooms.Length; i++) {
            Room room = _rooms[i];
            for (int r = 0; r < room.Height; r++) {
                for (int c = 0; c < room.Width; c++) {
                    _roomMap[room.Row + r, room.Col + c] = i;
                }
            }
        }

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

    private char GetMapChar(int row, int col) {
        if (OutOfMap(row, col)) return MapChar.Wall; // マップ外は壁扱い
        return _mapData[row, col];
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
        return GetMapChar(row, col) == MapChar.Wall;
    }

    public bool IsRoomOrPassage(Loc loc) {
        return IsRoomOrPassage(loc.Row, loc.Col);
    }

    public bool IsRoomOrPassage(int row, int col) {
        return IsRoom(row, col) || IsPassage(row, col);
    }

    public bool IsRoom(Loc loc) {
        return IsRoom(loc.Row, loc.Col);
    }

    public bool IsRoom(int row, int col) {
        return GetMapChar(row, col) == MapChar.Room;
    }

    public bool IsPassage(Loc loc) {
        return IsPassage(loc.Row, loc.Col);
    }

    public bool IsPassage(int row, int col) {
        return GetMapChar(row, col) == MapChar.Passage;
    }

    public bool IsSameRoom(Loc a, Loc b) {
        if (IsRoom(a) && IsRoom(b)) {
            return _roomMap[a.Row, a.Col] == _roomMap[b.Row, b.Col];
        }
        return false;
    }

    public Room FindRoom(Loc loc) {
        int no = _roomMap[loc.Row, loc.Col];
        Debug.LogFormat("FindRoom {0} no = {1}", loc, no);
        if (no != NoRoom) {
            return _rooms[no];
        }
        return null;
    }

    public Room[] GetRooms() {
        if (_rooms != null) return _rooms;

        var rooms = new List<Room>();
        var used = new bool[Rows, Cols];
        var delta = new List<int[]> {
            new[] { -1,  0 },
            new[] {  1,  0 },
            new[] {  0, -1 },
            new[] {  0,  1 },
        };

        for (int i = 0; i < Rows; i++) {
            for (int j = 0; j < Cols; j++) {
                if (used[i, j]) continue;

                if (_mapData[i, j] == MapChar.Room) {
                    int minR = i, maxR = i;
                    int minC = j, maxC = j;
                    var entrances = new List<Loc>();

                    var q = new Queue<Loc>();
                    q.Enqueue(new Loc(i, j));
                    while (q.Count > 0) {
                        Loc loc = q.Dequeue();

                        if (OutOfMap(loc)) continue;
                        if (used[loc.Row, loc.Col]) continue;
                        used[loc.Row, loc.Col] = true;

                        minR = Math.Min(minR, loc.Row);
                        maxR = Math.Max(maxR, loc.Row);
                        minC = Math.Min(minC, loc.Col);
                        maxC = Math.Max(maxC, loc.Col);

                        foreach (var d in delta) {
                            int r = loc.Row + d[0];
                            int c = loc.Col + d[1];
                            if (OutOfMap(r, c) || used[r, c]) continue;
                            if (_mapData[r, c] == MapChar.Room) {
                                q.Enqueue(new Loc(r, c));
                            }
                            else if (_mapData[r, c] == MapChar.Passage) {
                                entrances.Add(new Loc(r, c));
                            }
                        }
                    }

                    int width = maxC - minC + 1;
                    int height = maxR - minR + 1;
                    var room = new Room(minR, minC, width, height, entrances);
                    rooms.Add(room);
                }

            }
        }

        _rooms = rooms.ToArray();
        return _rooms;
    }
}
