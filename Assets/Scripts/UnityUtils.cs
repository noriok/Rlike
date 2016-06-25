using UnityEngine;
// using System.Collections;

public static class UnityUtils {
	// public static GameObject Inst(string path, Vector3 pos) {
	// 	var obj = Resources.Load(path);

	// 	var gobj = (GameObject)GameObject.Instantiate(obj);
	// 	gobj.transform.position = pos;
	// 	return gobj;
	// }

    public static GameObject Create(this GameObject prefab, Vector3 pos) {
        var o = (GameObject)GameObject.Instantiate(prefab, pos, Quaternion.identity);
        return o;
    }

    public static void SetAlpha(this GameObject obj, float a) {
        var renderer = obj.GetComponent<SpriteRenderer>();
        var color = renderer.color;
        color.a = a;
        renderer.color = color;
    }
}
