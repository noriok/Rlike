using UnityEngine;
using System;
// using System.Collections;
// using System.Linq;

public class MapSpotlight {
    private enum SpotlightState {
        None,    // スポットライトなし
        Passage, // 通路のスポットライト表示中
        Room,    // 部屋のスポットライト表示中
    }

    private SpotlightState _state = SpotlightState.None;

    private GameObject _spotlightPassageLayer;
    private GameObject _spotlightRoomLayer;

    private GameObject _passageSpotlight;
    private GameObject[,] _roomSpotlights;
    private GameObject[,] _roomCornerSpotlights; // 部屋の 4 隅(角が丸くなっている)

    private Room _prevVisitedRoom;

    public MapSpotlight(int rows, int cols, Room[] rooms) {
        _spotlightPassageLayer = LayerManager.GetLayer(LayerName.SpotlightPassage);
        _spotlightRoomLayer = LayerManager.GetLayer(LayerName.SpotlightRoom);

        // 通路のスポットライト
        var res = Resources.Load<GameObject>("Prefabs/Spotlight/spot");
        _passageSpotlight = res.Create(new Loc(0, 0).ToPosition());
        _passageSpotlight.SetAlpha(Config.SpotlightAlpha);
        _passageSpotlight.transform.SetParent(_spotlightPassageLayer.transform);

        // 部屋のスポットライト
        _roomSpotlights = new GameObject[rows, cols];
        _roomCornerSpotlights = new GameObject[rows, cols];

        // スポットライト 4 隅
        var nw = Resources.Load<GameObject>("Prefabs/Spotlight/round1");
        var ne = Resources.Load<GameObject>("Prefabs/Spotlight/round2");
        var se = Resources.Load<GameObject>("Prefabs/Spotlight/round3");
        var sw = Resources.Load<GameObject>("Prefabs/Spotlight/round4");
        foreach (Room room in rooms) {
            foreach (var a in room.OutsideCorners.Zip(new[] { nw, ne, se, sw }, (loc, obj) => new { loc, obj })) {
                _roomCornerSpotlights[a.loc.Row, a.loc.Col] = CreateRoomSpotlight(a.obj, a.loc.Row, a.loc.Col);
            }
        }

        var black40x40 = Resources.Load<GameObject>("Prefabs/Spotlight/black40x40");
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                 _roomSpotlights[i, j] = CreateRoomSpotlight(black40x40, i, j);

                if (_roomCornerSpotlights[i, j] != null) {
                    _roomCornerSpotlights[i, j].gameObject.SetActive(false);
                }
            }
        }
    }

    private GameObject CreateRoomSpotlight(GameObject prefab, int row, int col) {
        var spot = prefab.Create(new Loc(row, col).ToPosition());
        spot.SetAlpha(Config.SpotlightAlpha);
        spot.transform.SetParent(_spotlightRoomLayer.transform);
        return spot;
    }

    private void UpdateRoomSpotlight(Room room, bool isOn) {
        int rows = _roomSpotlights.GetLength(0);
        int cols = _roomSpotlights.GetLength(1);

        int rowFm = Math.Max(0, room.Row - 1);
        int rowTo = Math.Min(room.Row + room.Height, rows - 1);
        int colFm = Math.Max(0, room.Col - 1);
        int colTo = Math.Min(room.Col + room.Width, cols - 1);
        for (int r = rowFm; r <= rowTo; r++) {
            for (int c = colFm; c <= colTo; c++) {
                _roomSpotlights[r, c].gameObject.SetActive(!isOn);
            }
        }
        foreach (var loc in room.OutsideCorners) {
            _roomCornerSpotlights[loc.Row, loc.Col].gameObject.SetActive(isOn);
        }
    }

    // TODO:Rx プレイヤーの Position に合わせたい
    // ワープの場合は更新してはいけない
    public void UpdatePassageSpotlightPosition(Vector3 pos) {
        _passageSpotlight.transform.position = pos;
    }

    public void ActivatePassageSpotlight() {
        if (_state != SpotlightState.Passage) {
            _spotlightPassageLayer.SetActive(true);
            _spotlightRoomLayer.SetActive(false);
            _state = SpotlightState.Passage;
        }
    }

    public void ActivateRoomSpotlight(Room room) {
        if (_state != SpotlightState.Room) {
            _spotlightPassageLayer.SetActive(false);
            _spotlightRoomLayer.SetActive(true);
            _state = SpotlightState.Room;
        }

        if (_prevVisitedRoom == null || _prevVisitedRoom.Id != room.Id) {
            if (_prevVisitedRoom != null) {
                // 前回入った部屋と異なるなら、前回の部屋のスポットを消す
                UpdateRoomSpotlight(_prevVisitedRoom, false);
            }
            // 新しく入ったスポットを付ける
            UpdateRoomSpotlight(room, true);
            _prevVisitedRoom = room;
        }
    }
}
