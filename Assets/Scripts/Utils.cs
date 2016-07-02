using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public static class Utils {
    public static void Shuffle<T>(List<T> xs) {
        for (int i = 0; i < xs.Count; i++) {
            int p = Rand.Next(xs.Count);
            var t = xs[i];
            xs[i] = xs[p];
            xs[p] = t;
        }
    }

    public static Loc RandomRoomLoc(Room room) {
        int r = room.Row + Rand.Next(room.Height);
        int c = room.Col + Rand.Next(room.Width);
        return new Loc(r, c);
    }

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

    public static T Clamp<T>(T val, T low, T high) where T : IComparable<T> {
        if (val.CompareTo(low) < 0) return low;
        if (val.CompareTo(high) > 0) return high;
        return val;
    }

    // Unity utils

    public static T[,] CreateArray2D<T>(int rows, int cols, T val = default(T)) {
        var ary = new T[rows, cols];
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                ary[i, j] = val;
            }
        }
        return ary;
    }
}
