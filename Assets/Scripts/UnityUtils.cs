using UnityEngine;
using System.Collections;

public static class UnityUtils {
	public static GameObject Inst(string path, Vector3 pos) {
		var obj = Resources.Load(path);

		var gobj = (GameObject)GameObject.Instantiate(obj);
		gobj.transform.position = pos;
		return gobj;
	}

}
