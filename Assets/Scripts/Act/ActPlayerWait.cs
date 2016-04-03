using UnityEngine;
// using System.Collections;

public class ActPlayerWait : Act {

    public ActPlayerWait(Player player) : base(player) {

    }

    protected override int GetPriority() {
        return ActPriority.PlayerWait;
    }

    public override void RunEffect(MainSystem sys) {
        Debug.Log("@@@ player wait");
    }
}
