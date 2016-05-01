using UnityEngine;
// using System.Collections;

public class GameManager {
    public Player CreatePlayer(Loc loc) {
        // var obj = Resources.Load("Prefabs/Animations/kabocha_0");
        var obj = Resources.Load("Prefabs/Animations/majo_0");
        var gobj = (GameObject)GameObject.Instantiate(obj, Vector3.zero, Quaternion.identity);
        return new Player(loc, gobj);
    }
}
