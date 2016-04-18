// using UnityEngine;
// using System.Collections;

public class ActEnemyWait : Act {

    public ActEnemyWait(Enemy enemy) : base(enemy) {

    }

    public override void Apply(MainSystem sys) {
        DLog.D("{0} wait", Actor);
    }
}
