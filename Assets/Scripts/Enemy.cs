using UnityEngine;

public class Enemy : CharacterBase {
    public Enemy(int row, int col, GameObject gobj) : base(row, col, gobj) {

    }

    public override string ToString() {
        return string.Format("E:{0}", Loc);
    }
}
