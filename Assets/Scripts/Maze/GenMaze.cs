using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

public static class GenMaze {

    public static char[,] GenMazeRoom(int rows, int cols) {
		Assert.IsTrue(rows % 2 == 1 && cols % 2 == 1);
        if (!(rows % 2 == 1 && cols % 2 == 1)) throw new ArgumentException();

        var maze = new char[rows, cols];
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                if (i % 2 == 1 && j % 2 == 1) {
                    maze[i, j] = MapChar.Wall;
                }
                else {
                    maze[i, j] = MapChar.Room;
                }
            }
        }

        var ds = new List<int[]> {
            new[] {  1,  0 },
            new[] {  0, -1 },
            new[] {  0,  1 },
            new[] { -1,  0 },
        };

        // 最初の一行を倒す
        for (int i = 1; i < cols; i += 2) {
            Utils.Shuffle(ds);
            foreach (var d in ds) {
                int r = 1 + d[0];
                int c = i + d[1];
                if (maze[r, c] == MapChar.Room) {
                    maze[r, c] = MapChar.Wall;
                    break;
                }
            }
        }

        // 棒を上に倒せないので、上方向を取り除く
        ds = ds.Where(e => !(e[0] == -1 && e[1] == 0)).ToList();

        // 2 行目以降の棒を倒す
        for (int i = 3; i < rows; i += 2) {
            for (int j = 1; j < cols; j += 2) {
                Utils.Shuffle(ds);
                foreach (var d in ds) {
                    int r = i + d[0];
                    int c = j + d[1];
                    if (maze[r, c] == MapChar.Room) {
                        maze[r, c] = MapChar.Wall;
                        break;
                    }
                }
            }
        }
        return maze;
    }
}
