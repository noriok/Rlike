using UnityEngine;
// using System.Collections;

public class ActPlayerWait : Act {

    public ActPlayerWait(Player player) : base(player) {

    }

    public override void RunEffect(MainSystem sys) {
        Debug.Log("@@@ player wait");
        DLog.D("{0} wait", Actor);
    }
}
