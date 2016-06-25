using UnityEngine;
// using System.Collections;

public class Res {
    public static GameObject Bless(string path) {
        var prefab = Resources.Load<GameObject>(path);
        return prefab.Create();
    }
}
