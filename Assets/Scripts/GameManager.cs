// using UnityEngine;
// using System.Collections;

public class GameManager {
    public Player CreatePlayer(Loc loc) {
        // var gobj = Utils.Instantiate("Prefabs/Animations/majo_0");
        var gobj = Utils.Instantiate("Prefabs/Animations/player/FSM_03-A_05_0-S");
        return new Player(loc, gobj);
    }
}
