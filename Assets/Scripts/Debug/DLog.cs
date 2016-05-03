// using UnityEngine;
// using System.Collections;
// using System;
using System.Collections.Generic;

public static class DLog {
    public static bool Enable { get; set; }

    private static List<string> _log = new List<string>();

    public static void Clear() {
        _log.Clear();
    }

    public static void D(string format, params object[] args) {
        if (!Enable) return;

        _log.Add(string.Format(format, args));
    }

    public static string ToText() {
        return string.Join("\n", _log.ToArray());
    }
}
