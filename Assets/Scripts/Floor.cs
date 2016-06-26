using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Floor {
	private Map _map;
	private Minimap _minimap;
	private List<FieldObject> _fieldObjects;

    public int Rows { get { return _map.Rows; } }
    public int Cols { get { return _map.Cols; } }
    public Loc StairsLoc { get; private set; }

	public Floor(Map map, Minimap minimap, List<FieldObject> fieldObjects, Loc stairsLoc) {
		_map = map;
		_minimap = minimap;
		_fieldObjects = fieldObjects;
        StairsLoc = stairsLoc;
	}

    public void UpdateMinimapIconAll(Loc playerLoc, List<Enemy> enemies, List<FieldItem> items) {
        _minimap.UpdateIconAll(playerLoc, enemies, items);
    }

    public void UpdateMinimapEnemyIcon(List<Enemy> enemies) {
        _minimap.UpdateEnemyIcon(enemies);
    }

    public void UpdateMinimapPlayerIconBlink() {
        _minimap.UpdatePlayerIconBlink();
    }

    public void ShowMinimap() {
        _minimap.Show();
    }

    public void HideMinimap() {
        _minimap.Hide();
    }

    // 水がれ
    public IEnumerator Sun(Loc playerLoc) {
        var mapData = _map.MapData;
        LayerManager.RemoveLayer(LayerName.Map);
        LayerManager.RemoveLayer(LayerName.Minimap);
        LayerManager.RemoveLayer(LayerName.Spotlight);
        yield return null;
        LayerManager.CreateLayer(LayerName.Map);
        LayerManager.CreateLayer(LayerName.Minimap);
        LayerManager.CreateLayer(LayerName.Spotlight);

        for (int i = 0; i < mapData.GetLength(0); i++) {
            for (int j = 0; j < mapData.GetLength(1); j++) {
                if (mapData[i, j] == MapChar.Water) {
                    mapData[i, j] = MapChar.Sand;
                }
            }
        }

        _map = new Map(mapData);
        _minimap = new Minimap(mapData, _fieldObjects, StairsLoc);
        _map.UpdatePassageSpotlightPosition(playerLoc.ToPosition());
        _map.UpdateSpotlight(playerLoc);
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

    public bool ExistsTreasure(Loc loc) {
        return FindTreasure(loc) != null;
    }

    public NoticeBoard FindNoticeBoard(Loc loc) {
        return FindFieldObject<NoticeBoard>(loc);
    }

    public bool ExistsNoticeBoard(Loc loc) {
        return FindNoticeBoard(loc) != null;
    }

    public Trap FindTrap(Loc loc) {
        return FindFieldObject<Trap>(loc);
    }

    public bool ExistsTrap(Loc loc) {
        return FindTrap(loc) != null;
    }

	public bool CanAdvance(Loc fm, Dir dir) {
        Loc to = fm.Forward(dir);
        if (!(_map.IsRoomOrPassage(fm) && _map.IsRoomOrPassage(to))) return false;

        // 障害物が配置されているなら進めない
        if (ExistsObstacle(to)) return false;

        if (fm.IsDiagonal(to)) { // 斜め移動のときは、左右に障害物があるなら進めない
            if (_map.IsWall(fm.Row, to.Col) || _map.IsWall(to.Row, fm.Col)) {
                return false;
            }
            return true;
        }
        return true; // to に移動できる
	}

    // FieldObject かつ Obstacle なオブジェクト
    public bool ExistsObstacle(Loc loc) {
        foreach (var obj in _fieldObjects) {
            if (loc == obj.Loc && obj.IsObstacle()) {
                return true;
            }
        }
        return false;
    }

    public Room FindRoom(Loc loc) {
        return _map.FindRoom(loc);
    }

    public Room[] GetRooms() {
        return _map.GetRooms();
    }

    // fm から to が見えるか
    public bool InSight(Loc fm, Loc to) {
        if (fm.IsNeighbor(to)) return true;

        if (_map.IsSameRoom(fm, to)) return true;

        // fm が部屋内で、to がその部屋の入り口なら視界内
        Room room = _map.FindRoom(fm);
        if (room != null) {
            return room.IsEntrance(to);
        }

        // TODO:fm, to がいずれも通路なら 2 マス先まで視界内
        return false;
    }

    public bool IsRoom(Loc loc) {
        return _map.IsRoom(loc);
    }

    public bool IsPassage(Loc loc) {
        return _map.IsPassage(loc);
    }

    public bool IsWater(Loc loc) {
        return _map.IsWater(loc);
    }

    public bool IsEntrance(Loc loc) {
        return _map.IsEntrance(loc);
    }

    public bool IsRoomOrPassage(Loc loc) {
        return _map.IsRoomOrPassage(loc);
    }

    public bool IsWall(Loc loc) {
        return _map.IsWall(loc);
    }

    public bool CanPutItem(Loc loc) {
        if (!IsRoomOrPassage(loc)) return false;
        // 水の上にはアイテムは置けない
        if (IsWater(loc)) return false;

        // ワナが配置してあるなら置けない
        if (ExistsTrap(loc)) return false;
        // 宝箱が配置してあるなら置けない
        if (ExistsTreasure(loc)) return false;
        // 立て札が配置してあるなら置けない
        if (ExistsNoticeBoard(loc)) return false;
        // 階段の上には置けない
        if (StairsLoc == loc) return false;

        return true;
    }

    // めぐすり
    public void Eyedrops() {
        foreach (var fobj in _fieldObjects) {
            if (fobj is Trap && !fobj.Visible) {
                fobj.Visible = true;
                _minimap.AddTrapIcon(fobj.Loc);
            }
        }
    }

    public void UpdateSpot(Loc loc) {
        _map.UpdateSpotlight(loc);
    }

    public void UpdatePassageSpotlightPosition(Vector3 pos) {
        _map.UpdatePassageSpotlightPosition(pos);
    }
}
