using UnityEngine;
using System.Collections.Generic;

public class Player : CharacterBase {
    private Loc _nextLoc; // 次のターンでの位置

    private Dictionary<Dir, GameObject> _dirs = new Dictionary<Dir, GameObject>();

    public Player(int row, int col, GameObject gobj) : base(row, col, gobj) {
        SyncCameraPosition();

        Hp = MaxHp = 99999;

        var pos = gobj.transform.position;
        float d = Config.ChipSize / 2;
        var n = UnityUtils.Inst("Prefabs/Dir/dir-N", new Vector3(pos.x, pos.y + d + d/3, pos.z));
        var ne = UnityUtils.Inst("Prefabs/Dir/dir-NE", new Vector3(pos.x + d, pos.y + d, pos.z));
        var e = UnityUtils.Inst("Prefabs/Dir/dir-e", new Vector3(pos.x + d, pos.y, pos.z));
        var se = UnityUtils.Inst("Prefabs/Dir/dir-se", new Vector3(pos.x + d, pos.y - d, pos.z));
        var s = UnityUtils.Inst("Prefabs/Dir/dir-s", new Vector3(pos.x, pos.y - d - d/3, pos.z));
        var sw = UnityUtils.Inst("Prefabs/Dir/dir-sw", new Vector3(pos.x - d, pos.y - d, pos.z));
        var w = UnityUtils.Inst("Prefabs/Dir/dir-w", new Vector3(pos.x - d, pos.y, pos.z));
        var nw = UnityUtils.Inst("Prefabs/Dir/dir-nw", new Vector3(pos.x - d, pos.y + d, pos.z));

        _dirs.Add(Dir.N, n);
        _dirs.Add(Dir.NE, ne);
        _dirs.Add(Dir.E, e);
        _dirs.Add(Dir.SE, se);
        _dirs.Add(Dir.S, s);
        _dirs.Add(Dir.SW, sw);
        _dirs.Add(Dir.W, w);
        _dirs.Add(Dir.NW, nw);
        foreach (var kv in _dirs) {
            kv.Value.transform.SetParent(gobj.transform);
            kv.Value.SetActive(false);
        }
    }

    private void SyncCameraPosition() {
        var camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        float cameraZ = camera.transform.position.z;
        float x = Position.x;
        float y = Position.y;
        camera.transform.position = new Vector3(x, y, cameraZ);
    }

    public override void ShowDirection(Dir dir) {
        HideDirection();
        _dirs[dir].SetActive(true);
    }

    public override void HideDirection() {
        foreach (var kv in _dirs) {
            kv.Value.SetActive(false);
        }
    }

    public override string ToString() {
        return string.Format("P: Loc:{0}", Loc);
    }
}
