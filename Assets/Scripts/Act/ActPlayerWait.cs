﻿// using UnityEngine;
// using System.Collections;

public class ActPlayerWait : Act {

    public ActPlayerWait(Player player) : base(player) {

    }

    public override void OnFinished(MainSystem sys) {
        DLog.D("{0} wait", Actor);
    }
}
