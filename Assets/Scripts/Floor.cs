using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Floor {
	private Map _map;
	private Minimap _minimap;
	private List<FieldObject> _fieldObjects;
	private Loc _stairsLoc;

    public int Rows { get { return _map.Rows; } }
    public int Cols { get { return _map.Cols; } }

	public Floor(Map map, Minimap minimap, List<FieldObject> fieldObjects, Loc stairsLoc) {
		_map = map;
		_minimap = minimap;
		_fieldObjects = fieldObjects;
		_stairsLoc = stairsLoc;
	}

    public void UpdateMinimap(Loc playerLoc, List<Enemy> enemies) {
        _minimap.UpdateIcon(playerLoc, enemies);
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

	public bool CanAdvance(Loc fm, Dir dir) {
        Loc to = fm.Forward(dir);
        if (!(_map.IsRoomOrPassage(fm) && _map.IsRoomOrPassage(to))) return false;

        // 障害物が配置されているか
        if (ExistsObstacleFieldObject(to)) return false;

        if (fm.Row != to.Row && fm.Col != to.Col) { // 斜め移動
            if (_map.IsRoomOrPassage(fm.Row, to.Col) && _map.IsRoomOrPassage(to.Row, fm.Col)) {
                return true;
            }
            return false;
        }

        return true;
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

    public Room FindRoom(Loc loc) {
        return _map.FindRoom(loc);
    }

    public Room[] GetRooms() {
        return _map.GetRooms();
    }

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
}
