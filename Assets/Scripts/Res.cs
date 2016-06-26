using UnityEngine;
// using System.Collections;

public class Res {
    public static GameObject Create(string path) {
        return Create(path, Vector3.zero);
    }

    public static GameObject Create(string path, Vector3 pos) {
        var prefab = Resources.Load<GameObject>(path);
        return prefab.Create(pos);
    }

    public static GameObject Load(string path) {
        return Resources.Load<GameObject>(path);
    }
}
