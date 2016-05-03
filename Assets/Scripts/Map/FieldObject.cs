// using UnityEngine;
using System.Collections;

public abstract class FieldObject {
    public Loc Loc { get; private set; }
    public bool Visible { get; set; }

    public FieldObject(Loc loc) {
        Loc = loc;
    }

    public virtual IEnumerator RunAnimation(CharacterBase sender, MainSystem sys) {
        yield break;
    }

    public virtual bool IsObstacle() {
        return true;
    }
}
