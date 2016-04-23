using UnityEngine;

public class Player : CharacterBase {
    private Loc _nextLoc; // 次のターンでの位置

    public Player(int row, int col, GameObject gobj) : base(row, col, gobj) {
        SyncCameraPosition();

        Hp = MaxHp = 10000000;
    }

    private void SyncCameraPosition() {
        var camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        float cameraZ = camera.transform.position.z;

        float x = Position.x;
        float y = Position.y;
        camera.transform.position = new Vector3(x, y, cameraZ);
    }

    public override string ToString() {
        return string.Format("P:{0}", Loc);
    }
}
