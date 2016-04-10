using UnityEngine;
using System.Collections;
using System.Linq;

public class Map {
    private const string TestMap = @"
##########
#........#
#........#
#........#
#........#
#####.####
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
                _map[i, j] = '.';
            }
            for (int j = 0; j < lines[i].Length; j++) {
                _map[i, j] = lines[i][j];
            }
        }

        Debug.Log(_map);

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
}
