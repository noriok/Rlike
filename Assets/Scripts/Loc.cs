using System;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.Assertions;

// immutable. 内部状態は変更しない
public struct Loc : IEquatable<Loc> {
    public int Row { get; private set; }
    public int Col { get; private set; }

    public Loc(int row, int col) {
        Row = row;
        Col = col;
    }

    public bool Equals(Loc o) {
        return Row == o.Row && Col == o.Col;
    }

    public override bool Equals(object o) {
        if (o == null) return false;
        return Equals((Loc)o);
    }

    public override int GetHashCode() {
        return Row ^ Col;
    }

    public static bool operator==(Loc a, Loc b) {
        return a.Equals(b);
    }

    public static bool operator!=(Loc a, Loc b) {
        return !a.Equals(b);
    }

    public static Loc operator+(Loc a, Loc b) {
        return new Loc(a.Row + b.Row, a.Col + b.Col);
    }

    public static Loc operator-(Loc a, Loc b) {
        return new Loc(a.Row - b.Row, a.Col - b.Col);
    }

    public override string ToString() {
        return string.Format("({0}, {1})", Row, Col);
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
    public Loc Forward(Dir front) {
        var delta = front.Delta();
        return new Loc(Row + delta.Row, Col + delta.Col);
    }

    public Loc[] Forwards3(Dir front) {
        return new[] {
            Forward(front),
            Forward(front.Clockwise()),
            Forward(front.Anticlockwise()),
        };
    }

    // dir 方向へ(なるべく)前進する Loc を返す。
    // (前方、斜め前方、側面方向の Loc を返す)
    public Loc[] Forwards5(Dir front) {
        return new[] {
            Forward(front),
            Forward(front.Clockwise()),
            Forward(front.Anticlockwise()),
            Forward(front.Clockwise().Clockwise()),
            Forward(front.Anticlockwise().Anticlockwise()),
        };
    }

  // dir 方向へ(なるべく)前進する Loc を返す。
    // (前方、斜め前方、側面方向の Loc を返す)
    public Loc[] Forwards5AndBackward(Dir front) {
        return new[] {
            Forward(front),
            Forward(front.Clockwise()),
            Forward(front.Anticlockwise()),
            Forward(front.Clockwise().Clockwise()),
            Forward(front.Anticlockwise().Anticlockwise()),
            Backward(front),
        };
    }

    // 斜め方向
    public Loc[] ForwardsDiagonally(Dir front) {
        return new[] {
            Forward(front.Clockwise()),
            Forward(front.Anticlockwise()),
        };
    }

    // 横方向
    public Loc[] Sides(Dir front) {
        return new[] {
            Forward(front.Clockwise().Clockwise()),
            Forward(front.Anticlockwise().Anticlockwise()),
        };
    }

    public Loc Backward(Dir front) {
        return Forward(front.Opposite());
    }

    public Loc Right(Dir front) {
        return Forward(front.Clockwise().Clockwise());
    }

    public Loc Left(Dir front) {
        return Forward(front.Anticlockwise().Anticlockwise());
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

    // Unity 上の position を返す
    public Vector3 ToPosition() {
        float x =  Col * Config.ChipSize;
        float y = -Row * Config.ChipSize;
        return new Vector3(x, y, 0);
    }
}
