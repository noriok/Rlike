using System;
using UnityEngine.Assertions;

// immutable. 内部状態は変更しない
public struct Loc {
    public int Row { get; private set; }
    public int Col { get; private set; }

    public Loc(int row, int col) {
        Row = row;
        Col = col;
    }

    // 隣接しているなら true
    public bool IsNeighbor(Loc loc) {
        int drow = Math.Abs(Row - loc.Row);
        int dcol = Math.Abs(Col - loc.Col);

        if ((drow == 1 && dcol == 1) ||
            drow + dcol == 1)
        {
            return true;
        }
        return false;
    }

    // to へ向いたときの方向
    public Dir Toward(Loc to) {
        Loc delta = to - this;
        int drow = delta.Row;
        int dcol = delta.Col;

        if (drow != 0) drow /= Math.Abs(drow);
        if (dcol != 0) dcol /= Math.Abs(dcol);

        Assert.IsFalse(drow == 0 && dcol == 0);
        if (drow == -1) {
            switch (dcol) {
            case -1: return Dir.NW;
            case  0: return Dir.N;
            case  1: return Dir.NE;
            }
        }
        else if (drow == 0) {
            switch (dcol) {
            case -1: return Dir.W;
            case  0: Assert.IsTrue(false); break;
            case  1: return Dir.E;
            }
        }
        else if (drow == 1) {
            switch (dcol) {
                case -1: return Dir.SW;
                case  0: return Dir.S;
                case  1: return Dir.SE;
            }
        }

        Assert.IsTrue(false);
        return Dir.N; // ここには到達しない
    }

    // dir 方向へ 1 歩進んだ位置
    public Loc Forward(Dir dir) {
        int drow = 0;
        int dcol = 0;
        switch (dir) {
        case Dir.N:  drow = -1; dcol =  0; break;
        case Dir.NE: drow = -1; dcol =  1; break;
        case Dir.E:  drow =  0; dcol =  1; break;
        case Dir.SE: drow =  1; dcol =  1; break;
        case Dir.S:  drow =  1; dcol =  0; break;
        case Dir.SW: drow =  1; dcol = -1; break;
        case Dir.W:  drow =  0; dcol = -1; break;
        case Dir.NW: drow = -1; dcol = -1; break;
        }
        return new Loc(Row + drow, Col + dcol);
    }

    public override string ToString() {
        return string.Format("({0}, {1})", Row, Col);
    }

    public static Loc operator+(Loc a, Loc b) {
        return new Loc(a.Row + b.Row, a.Col + b.Col);
    }

    public static Loc operator-(Loc a, Loc b) {
        return new Loc(a.Row - b.Row, a.Col - b.Col);
    }
}
