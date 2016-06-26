using UnityEngine;
// using System.Collections;

public class Res {
    public static GameObject Bless(string path) {
        return Bless(path, Vector3.zero);
    }

    public static GameObject Bless(string path, Vector3 pos) {
        var prefab = Resources.Load<GameObject>(path);
        return prefab.Create(pos);
    }
}
