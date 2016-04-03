public struct Loc {
    public int Row { get; private set; }
    public int Col { get; private set; }

    public Loc(int row, int col) {
        Row = row;
        Col = col;
    }

    public override string ToString() {
        return string.Format("({0}, {1})", Row, Col);
    }

    public static Loc operator+(Loc a, Loc b) {
        return new Loc(a.Row + b.Row, a.Col + b.Col);
    }

}
