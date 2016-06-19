// using UnityEngine;
// using System.Collections;

public class ActEnemyWait : Act {

    public ActEnemyWait(Enemy enemy) : base(enemy) {
        _animationFinished = true;
    }

    public override void OnFinished(MainSystem sys) {
        DLog.D("{0} wait", Actor);
    }
}
