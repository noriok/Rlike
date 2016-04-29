using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Floor {
	private Map _map;
	private Minimap _minimap;
	private List<FieldObject> _fieldObjects;
	private Loc _stairsLoc;

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
        if (!(_map.IsFloor(fm) && _map.IsFloor(to))) return false;

        // 障害物が配置されているか
        if (ExistsObstacleFieldObject(to)) return false;

        if (fm.Row != to.Row && fm.Col != to.Col) { // 斜め移動
            if (_map.IsFloor(fm.Row, to.Col) && _map.IsFloor(to.Row, fm.Col)) {
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

    public Room[] GetRooms() {
        return _map.GetRooms();
    }

}
