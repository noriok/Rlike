// using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections.Generic;

public static class Extensions {
    // Array

    public static T Choice<T>(this T[] ary) {
        Assert.IsTrue(ary != null && ary.Length > 0);
        return ary[Rand.Next(ary.Length)];
    }

    // Linq

    public static IEnumerable<TResult> Zip<T, U, TResult>(this IEnumerable<T> xs, IEnumerable<U> ys, Func<T, U, TResult> fn) {
        var iter1 = xs.GetEnumerator();
        var iter2 = ys.GetEnumerator();
        while (iter1.MoveNext() && iter2.MoveNext()) {
            yield return fn(iter1.Current, iter2.Current);
        }
    }
}
