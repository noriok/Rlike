using System;

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
