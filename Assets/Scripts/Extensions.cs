using UnityEngine;
using UnityEngine.Assertions;

public static class Extensions {
    private static System.Random _rand = new System.Random();

    // Array

    public static T Choice<T>(this T[] ary) {
        Assert.IsTrue(ary != null && ary.Length > 0);
        return ary[_rand.Next(ary.Length)];
    }
}
