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

    private IEnumerator MoveAnimation() {
        Actor.ChangeDir(Utils.ToDir(_drow, _dcol));
        var src = Actor.Position;

        var camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        float cameraZ = camera.transform.position.z;

        float duration = 0.4f;
        float elapsed = 0;
        float dx = _dcol * Config.ChipSize;
        float dy = _drow * Config.ChipSize;
        while (elapsed <= duration) {
            float x = Mathf.Lerp(src.x, src.x + dx, elapsed / duration);
            float y = Mathf.Lerp(src.y, src.y - dy, elapsed / duration);
            Actor.Position = new Vector3(x, y, 0);
            elapsed += Time.deltaTime;

            // カメラの位置も合わせる
            camera.transform.position = new Vector3(x, y + Config.CameraOffsetY , cameraZ);
            yield return null;
        }

        // 位置ずれしないように最終位置にセット
        float x2 = src.x + dx;
        float y2 = src.y - dy;
        Actor.Position = new Vector3(x2, y2, 0);
        camera.transform.position = new Vector3(x2, y2 + Config.CameraOffsetY, cameraZ);

        AnimationFinished = true;
    }

    public override void RunAnimation(MainSystem sys) {
        sys.StartCoroutine(MoveAnimation());
    }

    public override void RunEffect(MainSystem sys) {
        var nextLoc = Actor.Loc + new Loc(_drow, _dcol);
        DLog.D("{0} move {1} -> {2}", Actor, Actor.Loc, nextLoc);
        Actor.UpdateLoc(nextLoc);
    }
}
