using UnityEngine;
using System.Collections;

public class ActEnemyWalk {
    public bool Finished { get; private set; }
    private Enemy _enemy;
    private bool _started;

    public ActEnemyWalk(Enemy enemy) {
        _enemy = enemy;
    }

    public IEnumerator Move() {
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
        Finished = true;
    }

    // TODO: 抽象メソッド。サブクラスでオーバーライド
    public void Run(MainSystem sys) {
        sys.StartCoroutine(Move());
    }

    public void Exec(MainSystem sys) {
        if (_started) return;

        _started = true;
        Run(sys);
    }
}
