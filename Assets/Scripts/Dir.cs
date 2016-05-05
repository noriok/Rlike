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

    public static bool IsDiagonal(this Dir dir) {
        switch (dir) {
        case Dir.NE:
        case Dir.NW:
        case Dir.SE:
        case Dir.SW:
            return true;
        }
        return false;
    }

    public static Loc Delta(this Dir dir) {
        int drow = 0;
        int dcol = 0;
        switch (dir) {
        case Dir.N:  drow = -1; dcol =  0; break;
        case Dir.NE: drow = -1; dcol =  1; break;
        case Dir.E:  drow =  0; dcol =  1; break;
        case Dir.SE: drow =  1; dcol =  1; break;
        case Dir.S:  drow =  1; dcol =  0; break;
        case Dir.SW: drow =  1; dcol = -1; break;
        case Dir.W:  drow =  0; dcol = -1; break;
        case Dir.NW: drow = -1; dcol = -1; break;
        default:
            Assert.IsTrue(false);
            break;
        }
        return new Loc(drow, dcol);
    }
}
