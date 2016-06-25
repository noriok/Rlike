using UnityEngine;
using System;
using System.Collections;

public class MapSpotlight {
    private enum State {
        None,
        Passage,
        Room,
    }

    private State _state = State.None;

    private GameObject _spotlightPassageLayer;
    private GameObject _spotlightRoomLayer;

    private GameObject _passageSpotlight;
    private GameObject[,] _roomSpotlights;

    private Room _prevVisitedRoom;

    public MapSpotlight(int rows, int cols) {
        _spotlightPassageLayer = LayerManager.GetLayer(LayerName.SpotlightPassage);
        _spotlightRoomLayer = LayerManager.GetLayer(LayerName.SpotlightRoom);

        // 通路のスポットライト
        var res = Resources.Load<GameObject>("Prefabs/Spotlight/spot");
        _passageSpotlight = res.Create(new Loc(0, 0).ToPosition());
        _passageSpotlight.SetAlpha(Config.SpotlightAlpha);
        _passageSpotlight.transform.SetParent(_spotlightPassageLayer.transform);

        // 部屋のスポットライト
        _roomSpotlights = new GameObject[rows, cols];

        var black40x40 = Resources.Load<GameObject>("Prefabs/Spotlight/black40x40");
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                _roomSpotlights[i, j] = CreateRoomSpotlight(black40x40, i, j);
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
    }

    // TODO:Rx プレイヤーの Position に合わせたい
    // ワープの場合は更新してはいけない
    public void UpdatePassageSpotlightPosition(Vector3 pos) {
        _passageSpotlight.transform.position = pos;
    }

    public void ActivatePassageSpotlight() {
        if (_state != State.Passage) {
            _spotlightPassageLayer.SetActive(true);
            _spotlightRoomLayer.SetActive(false);
            _state = State.Passage;
        }
    }

    public void ActivateRoomSpotlight(Room room) {
        if (_state != State.Room) {
            _spotlightPassageLayer.SetActive(false);
            _spotlightRoomLayer.SetActive(true);
            _state = State.Room;
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

/*
        Room room = FindRoom(loc);
        if (room == null) {
            _spotRoomLayer.SetActive(false);
            _spotPassageLayer.SetActive(true);
        }
        else {
            // 差分更新
            if (_prevVisitedRoomId != room.Id) {
                // 前回入った部屋と異なるなら、前回の部屋のスポットを消す
                if (_prevVisitedRoomId != -1) {
                    UpdateSpotLight(GetRoom(_prevVisitedRoomId), false);
                }
                // 新しく入った部屋のスポットを付ける
                UpdateSpotLight(room, true);
                _prevVisitedRoomId = room.Id;
            }
            _spotPassageLayer.SetActive(false);
            _spotRoomLayer.SetActive(true);
        }*/
    }
}
