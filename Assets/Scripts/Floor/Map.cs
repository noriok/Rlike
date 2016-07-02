﻿using UnityEngine;
using UnityEngine.Assertions;
using System;
// using System.Collections;
using System.Collections.Generic;
// using System.Linq;

public class Map {
    private const int NoRoom = -1;
    public char[,] MapData { get; private set; }

    public int Rows { get { return MapData.GetLength(0); } }
    public int Cols { get { return MapData.GetLength(1); } }

    private GameObject _mapLayer;

    private Room[] _rooms; // TODOプロパティに変更
    private int[,] _roomMap;

    private MapSpotlight _spotlight;

    public Map(char[,] mapData) {
        MapData = mapData;
        int rows = mapData.GetLength(0);
        int cols = mapData.GetLength(1);
        _rooms = GetRooms();
        _spotlight = new MapSpotlight(rows, cols, _rooms);

        // _roomMap に各マスがどの部屋に所属しているのかを記録する
        _roomMap = Utils.CreateArray2D(rows, cols, NoRoom);
        foreach (var room in _rooms) {
            for (int r = 0; r < room.Height; r++) {
                for (int c = 0; c < room.Width; c++) {
                    _roomMap[room.Row + r, room.Col + c] = room.Id;
                }
            }
        }

        _mapLayer = LayerManager.GetLayer(LayerName.Map);
        var flat = Resources.Load("Prefabs/MapChip/pipo-map001_0");
        var mountain = Resources.Load("Prefabs/MapChip/pipo-map001_at-yama2_0");
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                if (MapData[i, j] == MapChar.Water || MapData[i, j] == MapChar.Sand) {
                    var dir = Dir.N;
                    var xs = new List<int>();
                    var loc = new Loc(i, j);
                    for (int k = 0; k < 8; k++) {
                        var chip = GetMapChar(loc.Forward(dir));
                        xs.Add(chip == MapData[i, j] ? 0 : 1); // 隣接するマップチップが同じか
                        dir = dir.Clockwise();
                    }
                    // Debug.Log(">> " + string.Join(" ", xs.Select(e => e.ToString()).ToArray()));

                    var offsets = new[,] { { -1, 1, }, { 1, 1 }, { -1, -1 }, { 1, -1 } };
                    var pathnames = MapData[i, j] == MapChar.Water
                        ? MapChipUtils.GetSeaMapChipName(xs.ToArray())
                        : MapChipUtils.GetSandMapChipName(xs.ToArray());

                    for (int k = 0; k < pathnames.Length; k++) {
                        // Debug.Log("pathname: " + pathnames[k]);
                        var pos = loc.ToPosition();

                        var obj = Resources.Load("Prefabs/MapChip/" + pathnames[k]);
                        pos.x += offsets[k, 0] * Config.ChipSize / 4;
                        pos.y += offsets[k, 1] * Config.ChipSize / 4;
                        var gobj = (GameObject)GameObject.Instantiate(obj, pos, Quaternion.identity);
                        gobj.transform.SetParent(_mapLayer.transform);
                    }
                }
                else {
                    var pos = new Loc(i, j).ToPosition();
                    var floatObj = (GameObject)GameObject.Instantiate(flat, pos, Quaternion.identity);
                    floatObj.transform.SetParent(_mapLayer.transform);

                    switch (MapData[i, j]) {
                    case MapChar.Wall:
                        var mtObj = (GameObject)GameObject.Instantiate(mountain, pos, Quaternion.identity);
                        mtObj.transform.SetParent(_mapLayer.transform);
                        break;
                    }
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

    private char GetMapChar(Loc loc) {
        return GetMapChar(loc.Row, loc.Col);
    }

    private char GetMapChar(int row, int col) {
        if (OutOfMap(row, col)) return MapChar.Wall; // マップ外は壁扱い
        return MapData[row, col];
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
        if (OutOfMap(row, col)) return false;

        if (_roomMap[row, col] != NoRoom) {
            char c = GetMapChar(row, col);
            if (c == MapChar.Room || c == MapChar.Sand) {
                return true;
            }
        }
        return false;
    }

    public bool IsPassage(Loc loc) {
        return IsPassage(loc.Row, loc.Col);
    }

    public bool IsPassage(int row, int col) {
        return GetMapChar(row, col) == MapChar.Passage;
    }

    public bool IsWater(Loc loc) {
        return IsWater(loc.Row, loc.Col);
    }

    public bool IsWater(int row, int col) {
        return GetMapChar(row, col) == MapChar.Water;
    }

    public bool IsSameRoom(Loc a, Loc b) {
        if (IsRoom(a) && IsRoom(b)) {
            return _roomMap[a.Row, a.Col] == _roomMap[b.Row, b.Col];
        }
        return false;
    }

    public bool IsEntrance(Loc loc) {
        for (int i = 0; i < _rooms.Length; i++) {
            if (Array.IndexOf(_rooms[i].Entrances, loc) >= 0) {
                return true;
            }
        }
        return false;
    }

    public Room FindRoom(Loc loc) {
        int no = _roomMap[loc.Row, loc.Col];
        if (no != NoRoom) {
            return _rooms[no];
        }
        return null;
    }

    private Room GetRoom(int roomId) {
        Assert.IsTrue(0 <= roomId && roomId < _rooms.Length);
        return _rooms[roomId];
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

                if (MapData[i, j] == MapChar.Room) {
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
                            if (MapData[r, c] == MapChar.Room) {
                                q.Enqueue(new Loc(r, c));
                            }
                            else if (MapData[r, c] == MapChar.Passage) {
                                entrances.Add(new Loc(r, c));
                            }
                        }
                    }

                    // 部屋の内部の孤島は部屋の一部と見なす
                    for (int k = minR; k <= maxR; k++) {
                        for (int m = minC; m <= maxC; m++) {
                            used[k, m] = true;
                        }
                    }

                    int width = maxC - minC + 1;
                    int height = maxR - minR + 1;
                    int id = rooms.Count;
                    var room = new Room(id, minR, minC, width, height, entrances);
                    rooms.Add(room);
                }
            }
        }
        return _rooms = rooms.ToArray();
    }

    public void UpdatePassageSpotlightPosition(Vector3 pos) {
        _spotlight.UpdatePassageSpotlightPosition(pos);
    }

    public void UpdateSpotlight(Loc loc) {
        Room room = FindRoom(loc);
        if (room == null) {
            _spotlight.ActivatePassageSpotlight();
        }
        else {
            _spotlight.ActivateRoomSpotlight(room);
        }
    }
}
