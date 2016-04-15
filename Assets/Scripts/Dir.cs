using UnityEngine.Assertions;

// 方向
public enum Dir {
    N,  // 北
    NE, // 北東
    E,  // 東
    SE, // 南東
    S,  // 南
    SW, // 南西
    W,  // 西
    NW, // 北西
}

public static class DirExtensions {
    public static Dir Clockwise(this Dir dir) {
        switch (dir) {
        case Dir.N:  return Dir.NE;
        case Dir.NE: return Dir.E;
        case Dir.E:  return Dir.SE;
        case Dir.SE: return Dir.S;
        case Dir.S:  return Dir.SW;
        case Dir.SW: return Dir.W;
        case Dir.W:  return Dir.NW;
        case Dir.NW: return Dir.N;
        }
        Assert.IsTrue(false);
        return Dir.N;
    }

    public static Dir Anticlockwise(this Dir dir) {
       switch (dir) {
        case Dir.N:  return Dir.NW;
        case Dir.NW: return Dir.W;
        case Dir.W:  return Dir.SW;
        case Dir.SW: return Dir.S;
        case Dir.S:  return Dir.SE;
        case Dir.SE: return Dir.E;
        case Dir.E:  return Dir.NE;
        case Dir.NE: return Dir.N;
        }
        Assert.IsTrue(false);
        return Dir.N;
    }

    public static Dir Opposite(this Dir dir) {
        return dir.Clockwise().Clockwise().Clockwise().Clockwise();
    }
}
