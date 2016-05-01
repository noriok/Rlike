// using UnityEngine;
// using System.Collections;

public class GameManager {
    public Player CreatePlayer(Loc loc) {
        var gobj = Utils.Instantiate("Prefabs/Animations/majo_0");
        return new Player(loc, gobj);
    }
}
