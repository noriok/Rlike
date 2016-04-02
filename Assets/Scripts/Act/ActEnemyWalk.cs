using UnityEngine;
using System.Collections;

public class ActEnemyWalk {
    private Enemy _enemy;

    public ActEnemyWalk(Enemy enemy) {
        _enemy = enemy;
    }

    public IEnumerator Exec() {
        var src = _enemy.Position;

        float duration = 1.0f;
        float elapsed = 0;
        while (elapsed <= duration) {
            float x = Mathf.Lerp(src.x, src.x + 3.0f, elapsed / duration);
            float y = Mathf.Lerp(src.y, src.y, elapsed / duration);
            _enemy.Position = new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
