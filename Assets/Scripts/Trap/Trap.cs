using UnityEngine;
// using System.Collections;

public abstract class Trap : SimpleFieldObject {

    public Trap(Loc loc, GameObject gobj) : base(loc, gobj) {
        Visible = false;
    }

    public override bool IsObstacle() {
        return false;
    }
}
