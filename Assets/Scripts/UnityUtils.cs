using UnityEngine;
// using System.Collections;

public static class UnityUtils {
    public static GameObject Create(this GameObject prefab) {
        return prefab.Create(Vector3.zero);
    }

    public static GameObject Create(this GameObject prefab, Vector3 pos) {
        // 回転させているプレハブがあるので引数には prefab のみ渡す
        // var o = (GameObject)GameObject.Instantiate(prefab, pos, Quaternion.identity);
        var o = (GameObject)GameObject.Instantiate(prefab);
        o.transform.position = pos;
        return o;
    }

    public static void SetAlpha(this GameObject obj, float a) {
        var renderer = obj.GetComponent<SpriteRenderer>();
        var color = renderer.color;
        color.a = a;
        renderer.color = color;
    }
}
