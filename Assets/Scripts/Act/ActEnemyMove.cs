using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class ActEnemyMove : Act {
    private int _drow;
    private int _dcol;

    // nextLoc = 移動後の位置
    public ActEnemyMove(Enemy enemy, Loc nextLoc) : base(enemy) {
        Assert.IsTrue(enemy.Loc.IsNeighbor(nextLoc));

        _drow = nextLoc.Row - enemy.Row;
        _dcol = nextLoc.Col - enemy.Col;
    }

    private IEnumerator MoveAnimation() {
        Actor.ChangeDir(Utils.ToDir(_drow, _dcol));

        var src = Actor.Position;

        float duration = 0.4f;
        float elapsed = 0;
        float dx = _dcol * Config.ChipSize;
        float dy = _drow * Config.ChipSize;
        while (elapsed <= duration) {
            float x = Mathf.Lerp(src.x, src.x + dx, elapsed / duration);
            float y = Mathf.Lerp(src.y, src.y - dy, elapsed / duration);
            Actor.Position = new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 位置ずれしないように最終位置にセット
        float x2 = src.x + dx;
        float y2 = src.y - dy;
        Actor.Position = new Vector3(x2, y2, 0);

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
