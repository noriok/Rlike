using UnityEngine;
using System.Collections;

public class ActEnemyWalk : Act { // rename walk -> move
    public ActEnemyWalk(Enemy enemy) : base(enemy) {
    }

    private IEnumerator MoveAnimation() {
        var src = Actor.Position;

        float duration = 0.4f;
        float elapsed = 0;
        while (elapsed <= duration) {
            float x = Mathf.Lerp(src.x, src.x + 1.0f, elapsed / duration);
            float y = Mathf.Lerp(src.y, src.y, elapsed / duration);
            Actor.Position = new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        AnimationFinished = true;
    }

    protected override int GetPriority() {
        return ActPriority.Move;
    }

    public override void RunAnimation(MainSystem sys) {
        sys.StartCoroutine(MoveAnimation());
    }

    public override void RunEffect(MainSystem sys) {
    }
}
