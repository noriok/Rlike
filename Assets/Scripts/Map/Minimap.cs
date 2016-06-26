using UnityEngine;
// using UnityEngine.Assertions;
// using System.Collections;
using System.Collections.Generic;

// TODO:ワナアイコンの Visible 管理
public class Minimap {
    private const float Size = 0.32f; // 32x32

    private GameObject _layer;

    private GameObject _playerIcon;
    private List<GameObject> _enemyIcons = new List<GameObject>();
    private List<GameObject> _itemIcons = new List<GameObject>();

    private float _elapsed;

    public Minimap(char[,] map, List<FieldObject> fieldObjects, Loc stairsLoc) {
        _layer = LayerManager.GetLayer(LayerName.Minimap);

        int rows = map.GetLength(0);
        int cols = map.GetLength(1);
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                switch (map[i, j]) {
                case MapChar.Room:
                case MapChar.Passage:
                case MapChar.Sand:
                    CreateFloor(i, j);
                    break;
                case MapChar.Water: // TODO:水
                    CreateWater(i, j);
                    break;
                }
            }
        }

        // trap
        foreach (var fo in fieldObjects) {
            if (fo is Trap && fo.Visible) {
                CreateTrapIcon(fo.Loc.Row, fo.Loc.Col);
            }
        }

        // 階段
        CreateStairsIcon(stairsLoc.Row, stairsLoc.Col);

        // プレイヤー
        _playerIcon = CreatePlayerIcon(0, 0);

        _layer.transform.localScale = new Vector3(0.1f, 0.1f, 1.0f);
        _layer.transform.position = new Vector3(-0.8f, 0.3f, 0);
    }

    public void AddTrapIcon(Loc loc) {
        CreateTrapIcon(loc.Row, loc.Col);
    }

    public void UpdateIcon(Loc playerLoc, List<Enemy> enemies, List<FieldItem> items) {
        _playerIcon.transform.localPosition = ToMinimapPosition(playerLoc);

        // いったん敵アイコンを全てオフにする
        for (int i = 0; i < _enemyIcons.Count; i++) {
            _enemyIcons[i].SetActive(false);
        }
        // いったんアイテムアイコンを全てオフにする
        for (int i = 0; i < _itemIcons.Count; i++) {
            _itemIcons[i].SetActive(false);
        }

        // 敵アイコンの更新
        int p = 0;
        for (int i = 0; i < enemies.Count; i++) {
            if (enemies[i].IsInvisible()) continue;

            if (p == _enemyIcons.Count) { // 追加
                var loc = enemies[i].Loc;
                _enemyIcons.Add(CreateEnemyIcon(loc.Row, loc.Col));
            }
            else {
                _enemyIcons[p].SetActive(true);
                _enemyIcons[p].transform.localPosition = ToMinimapPosition(enemies[i].Loc);
            }
            p++;
        }

        // アイテムアイコンの更新
        for (int i = 0; i < items.Count; i++) {
            if (i >= _itemIcons.Count) { // 追加
                var loc = items[i].Loc;
                _itemIcons.Add(CreateItemIcon(loc.Row, loc.Col));
            }
            else {
                _itemIcons[i].SetActive(true);
                _itemIcons[i].transform.localPosition = ToMinimapPosition(items[i].Loc);
            }
        }
    }

    // プレイヤーアイコンの点滅更新
    public void UpdatePlayerIconBlink() {
        float blinkTime = 0.5f;
        _elapsed += Time.deltaTime;
        if (_elapsed >= blinkTime) {
            _playerIcon.SetActive(!_playerIcon.activeSelf);
            _elapsed -= blinkTime;
        }
    }

    public void Show() {
        _layer.SetActive(true);
    }

    public void Hide() {
        _layer.SetActive(false);
    }

    private Vector3 ToMinimapPosition(Loc loc) {
        return new Vector3(loc.Col * Size, -loc.Row * Size, 0);
    }

    private GameObject CreateIcon(string path, int row, int col) {
        var pos = ToMinimapPosition(new Loc(row, col));
        var obj = Res.Bless(path, pos);
        obj.transform.SetParent(_layer.transform, false);
        return obj;
    }

    private GameObject CreateFloor(int row, int col) {
        var gobj = CreateIcon("Prefabs/Minimap/minimap-floor", row, col);
        var renderer = gobj.GetComponent<SpriteRenderer>();
        var color = renderer.color;
        color.a = 140f / 255f;
        renderer.color = color;
        return gobj;
    }

    private GameObject CreateWater(int row, int col) {
        var gobj = CreateIcon("Prefabs/Minimap/minimap-water", row, col);
        var renderer = gobj.GetComponent<SpriteRenderer>();
        var color = renderer.color;
        color.a = 0.4f;
        renderer.color = color;
        return gobj;
    }

    private GameObject CreateTrapIcon(int row, int col) {
        return CreateIcon("Prefabs/Minimap/minimap-trap", row, col);
    }

    private GameObject CreatePlayerIcon(int row, int col) {
        return CreateIcon("Prefabs/Minimap/minimap-player", row, col);
    }

    private GameObject CreateEnemyIcon(int row, int col) {
        return CreateIcon("Prefabs/Minimap/minimap-enemy", row, col);
    }

    private GameObject CreateItemIcon(int row, int col) {
        return CreateIcon("Prefabs/Minimap/minimap-item", row, col);
    }

    private GameObject CreateStairsIcon(int row, int col) {
        return CreateIcon("Prefabs/Minimap/minimap-stairs", row, col);
    }
}
