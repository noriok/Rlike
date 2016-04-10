using UnityEngine;

public class Player : CharacterBase {
    private Loc _nextLoc; // 次のターンでの位置

    public Player(int row, int col, GameObject gobj) : base(row, col, gobj) {

    }

    public override string ToString() {
        return string.Format("P:{0}", Loc);
    }
}
