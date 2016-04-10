using UnityEngine;
// using System.Collections;

public class ActEnemyWait : Act {

    public ActEnemyWait(Enemy enemy) : base(enemy) {

    }

    public override void RunEffect(MainSystem sys) {
        Debug.Log("@@@ enemy wait");
        DLog.D("{0} wait", Actor);
    }

}
