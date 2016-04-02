public struct Loc {
    public int Row { get; private set; }
    public int Col { get; private set; }

    public Loc(int row, int col) {
        Row = row;
        Col = col;
    }
}
