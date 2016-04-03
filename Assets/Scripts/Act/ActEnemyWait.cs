using UnityEngine;
// using System.Collections;

public class ActEnemyWait : Act {

    public ActEnemyWait(Enemy enemy) : base(enemy) {

    }

    protected override int GetPriority() {
        return ActPriority.EnemyWait;
    }

    public override void RunEffect(MainSystem sys) {
        Debug.Log("@@@ enemy wait");
    }

}
