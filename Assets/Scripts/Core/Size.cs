public struct Size {
    public int Rows { get; private set; }
    public int Cols { get; private set; }

    public Size(int rows, int cols) {
        Rows = rows;
        Cols = cols;
    }

    public bool IsInside(Loc loc) {
        return 0 <= loc.Row && loc.Row < Rows && 0 <= loc.Col && loc.Col < Cols;
    }
}
