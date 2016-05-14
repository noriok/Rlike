using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public static class MapChipUtils {

    private const int x = -1;
    // 1 が陸, 0 が海
    // Dir.N から時計回り
    private static int[,] _xs = {
        // 1
        { 1, x, x, x, x, x, 1, x }, // 4マスのうち左上
        { 1, x, 1, x, x, x, x, x }, // 右上
        { x, x, x, x, 1, x, 1, x }, // 左下
        { x, x, 1, x, 1, x, x, x }, // 右下

        // 2
        { 0, x, x, x, x, x, 1, x },
        { 0, x, 1, x, x, x, x, x },
        { x, x, x, x, 0, x, 1, x },
        { x, x, 1, x, 0, x, x, x },

        // 3
        { 1, x, x, x, x, x, 0, x },
        { 1, x, 0, x, x, x, x, x },
        { x, x, x, x, 1, x, 0, x },
        { x, x, 0, x, 1, x, x, x },

        // 4
        { 0, x, x, x, x, x, 0, 1 },
        { 0, 1, 0, x, x, x, x, x },
        { x, x, x, x, 0, 1, 0, x },
        { x, x, 0, 1, 0, x, x, x },

        // 5
        { 0, x, x, x, x, x, 0, 0 },
        { 0, 0, 0, x, x, x, x, x },
        { x, x, x, x, 0, 0, 0, x },
        { x, x, 0, 0, 0, x, x, x },
    };

    private static string[] GetMapChipName(int[] neighbors, string pathNamePrefix) {
        var xs = new List<string>();
        for (int i = 0; i < 4; i++) {

            bool ok = false;
            for (int j = 0; j < 5; j++) {
                int p = i + j * 4;

                bool matched = true;
                for (int k = 0; k < 8; k++) {
                    if (_xs[p, k] == x) continue;
                    if (_xs[p, k] != neighbors[k]) {
                        matched = false;
                        break;
                    }
                }

                if (matched) {
                    // var name = string.Format("pipo-map001_at-umi_{0}", p);
                    var name = string.Format("{0}{1}", pathNamePrefix, p);
                    xs.Add(name);
                    ok = true;
                    break;
                }
            }

            // Assert.IsTrue(ok);
            if (!ok) {
                Debug.LogWarning("マップチップが見つかりません");
                xs.Add("pipo-map001_at-umi_19");
            }
        }
        return xs.ToArray();
    }

    public static string[] GetSeaMapChipName(int[] neighbors) {
        return GetMapChipName(neighbors, "pipo-map001_at-umi_");
    }

    public static string[] GetSandMapChipName(int[] neighbors) {
        return GetMapChipName(neighbors, "pipo-map001_at-sabaku_");
    }


}
