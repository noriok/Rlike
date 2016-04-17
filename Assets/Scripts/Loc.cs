using System;
using System.Collections.Generic;
// using UnityEngine.Assertions;

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

    // to への方向
    public Dir Toward(Loc to) {
        Loc delta = to - this;
        int drow = delta.Row;
        int dcol = delta.Col;

        if (drow != 0) drow /= Math.Abs(drow);
        if (dcol != 0) dcol /= Math.Abs(dcol);

        return Utils.ToDir(drow, dcol);
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

    // dir 方向へ(なるべく)前進する Loc を返す。
    // (前方、斜め前方、側面方向の Loc を返す)
    public Loc[] Forwards(Dir dir) {
        return new[] {
            Forward(dir),
            Forward(dir.Clockwise()),
            Forward(dir.Anticlockwise()),
            Forward(dir.Clockwise().Clockwise()),
            Forward(dir.Anticlockwise().Anticlockwise()),
        };
    }

    // 斜め方向
    public Loc[] ForwardsDiagonally(Dir dir) {
        return new[] {
            Forward(dir.Clockwise()),
            Forward(dir.Anticlockwise()),
        };
    }

    // 横方向
    public Loc[] ForwardsCrossly(Dir dir) {
        return new[] {
            Forward(dir.Clockwise().Clockwise()),
            Forward(dir.Anticlockwise().Anticlockwise()),
        };
    }

    public Loc Backward(Dir dir) {
        return Forward(dir.Opposite());
    }

    public Loc[] Neighbors() {
        var locs = new List<Loc>();
        for (int i = -1; i <= 1; i++) {
            for (int j = -1; j <= 1; j++) {
                if (Math.Abs(i) + Math.Abs(j) > 0) {
                    locs.Add(new Loc { Row = Row + i, Col = Col + j});
                }
            }
        }
        return locs.ToArray();
    }

    // loc との距離の 2 乗を返す
    public int SquareDistance(Loc loc) {
        int drow = Row - loc.Row;
        int dcol = Col - loc.Col;
        return drow * drow + dcol * dcol;
    }

    public int ManhattanDistance(Loc loc) {
        return Math.Abs(Row - loc.Row) + Math.Abs(Col - loc.Col);
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
