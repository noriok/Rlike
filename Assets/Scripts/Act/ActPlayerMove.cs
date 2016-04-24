using UnityEngine;
using System.Collections;

public class ActPlayerMove : Act {
    private int _drow;
    private int _dcol;

    public ActPlayerMove(Player player, int drow, int dcol) : base(player) {
        _drow = drow;
        _dcol = dcol;
    }

    public override bool IsMoveAct() {
        return true;
    }

    protected override IEnumerator RunAnimation(MainSystem sys) {
        Actor.ChangeDir(Utils.ToDir(_drow, _dcol));

        // TODO: 一歩一歩が連続していないので矢印が点滅してしまう
        //Actor.ShowDirection(Utils.ToDir(_drow, _dcol));

        var camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        float cameraZ = camera.transform.position.z;
        yield return CAction.Walk(Actor, _drow, _dcol, (x, y) => {
            // カメラの位置も合わせる
            camera.transform.position = new Vector3(x, y + Config.CameraOffsetY, cameraZ);
        });

        // Actor.HideDirection();
    }

    public override void Apply(MainSystem sys) {
        var nextLoc = Actor.Loc + new Loc(_drow, _dcol);
        DLog.D("{0} move {1} -> {2}", Actor, Actor.Loc, nextLoc);
        Actor.UpdateLoc(nextLoc);
    }
}
