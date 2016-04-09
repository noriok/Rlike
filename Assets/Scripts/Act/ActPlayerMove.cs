using UnityEngine;
using System.Collections;

public class ActPlayerMove : Act {
    private int _drow;
    private int _dcol;

    public ActPlayerMove(Player player, int drow, int dcol) : base(player) {
        _drow = drow;
        _dcol = dcol;
    }

    private IEnumerator MoveAnimation() {
        var src = Actor.Position;

        float duration = 0.4f;
        float elapsed = 0;
        while (elapsed <= duration) {
            float x = Mathf.Lerp(src.x, src.x + _dcol, elapsed / duration);
            float y = Mathf.Lerp(src.y, src.y - _drow, elapsed / duration);
            Actor.Position = new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        AnimationFinished = true;
    }

    public override void RunAnimation(MainSystem sys) {
        sys.StartCoroutine(MoveAnimation());
    }

    public override void RunEffect(MainSystem sys) {
        var nextLoc = Actor.Loc + new Loc(_drow, _dcol);
        Debug.LogFormat("@@@ player move {0} -> {1}", Actor.Loc, nextLoc);
        Actor.UpdateLoc(nextLoc);
    }
}
