using System.Collections.Generic;
// using UnityEngine;
using UnityEngine.Assertions;

public static class Utils {

    public static Dir ToDir(int drow, int dcol) {
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
        return Dir.N;
    }

    public static int[] Digits(int n) {
        Assert.IsTrue(n >= 0);
        if (n == 0) return new[] { 0 };

        var xs = new List<int>();
        int x = n;
        while (x > 0) {
            xs.Add(x % 10);
            x /= 10;
        }
        xs.Reverse();
        return xs.ToArray();
    }
}
