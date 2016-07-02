using System;

public static class Rand {
    private static Random _rand = new Random();

    public static int Next(int maxValue) {
        return _rand.Next(maxValue);
    }

    public static int Next(int minValue, int maxValue) {
        return _rand.Next(minValue, maxValue);
    }
}
