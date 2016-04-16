using UnityEngine;
// using System.Collections;
using System.Linq;

public class Map {
    private const string TestMap = @"
#######################
#.....................#
#.....................#
#.....................#
#.....................#
#####.#################
    #.#
    #.#    ##############
    #.#    #............#
    #.######............#
    #...................#
    ########............#
           #............#
           ##############
";
    private char[,] _map;

    public int Rows { get { return _map.GetLength(0); }}
    public int Cols { get { return _map.GetLength(1); }}

    public Map() {
        var lines = TestMap.Trim().Split(new[] { '\n' });

        int rows = lines.Length;
        int cols = lines.Select(s => s.Length).Max();

        _map = new char[rows, cols];
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                _map[i, j] = MapChar.Floor;
            }
            for (int j = 0; j < lines[i].Length; j++) {
                _map[i, j] = lines[i][j];
            }
        }

        var flat = Resources.Load("Prefabs/MapChip/pipo-map001_0");
        var mountain = Resources.Load("Prefabs/MapChip/pipo-map001_at-yama2_0");
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                float x = j * Config.ChipSize;
                float y = i * Config.ChipSize;

                var pos = new Vector3(x, -y, 0);
                GameObject.Instantiate(flat, pos, Quaternion.identity);
                switch (_map[i, j]) {
                case '#':
                    GameObject.Instantiate(mountain, pos, Quaternion.identity);
                    break;
                }
            }
        }
    }

    public bool OutOfMap(Loc loc) {
        return OutOfMap(loc.Row, loc.Col);
    }

    public bool OutOfMap(int row, int col) {
        return row < 0 || row >= Rows || col < 0 || col >= Cols;
    }

    public bool IsWall(Loc loc) {
        return IsWall(loc.Row, loc.Col);
    }

    public bool IsWall(int row, int col) {
        if (OutOfMap(row, col)) return true; // マップ外は壁扱い
        return _map[row, col] == MapChar.Wall;
    }

    public bool IsFloor(Loc loc) {
        return IsFloor(loc.Row, loc.Col);
    }

    public bool IsFloor(int row, int col) {
        if (OutOfMap(row, col)) return false;
        return _map[row, col] == MapChar.Floor;
    }

    // fm から dir に向かって前進できるか
    public bool CanAdvance(Loc fm, Dir dir) {
        Loc to = fm.Forward(dir);
        if (!(IsFloor(fm) && IsFloor(to))) return false;

        if (fm.Row != to.Row && fm.Col != to.Col) {
            // 斜め方向の移動

            if (IsFloor(fm.Row, to.Col) && IsFloor(to.Row, fm.Col)) {
                return true;
            }
            return false;
        }

        return true;
    }
}
