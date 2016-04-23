﻿using UnityEngine;
using System.Collections;

public static class FieldObjectFactory {
	private static GameObject Create(string path, Vector3 pos, GameObject layer) {
		var gobj = (GameObject)GameObject.Instantiate(Resources.Load(path), pos, Quaternion.identity);
		gobj.transform.SetParent(layer.transform);
		return gobj;
	}

	public static Bonfire CreateBonfire(Loc loc, GameObject layer) {
		var obj = Create("Prefabs/MapObject/bonfire", loc.ToPosition(), layer);
		return new Bonfire(loc, obj);
	}

	public static Treasure CreateTreasure(Loc loc, GameObject layer) {
		var pos = loc.ToPosition();

		var open = Create("Prefabs/MapObject/treasure-open", pos, layer);
		var close = Create("Prefabs/MapObject/treasure-close", pos, layer);
		var anim = Create("Prefabs/Animations/treasure-open-anim", pos, layer);
		return new Treasure(loc, open, close, anim);
	}
}
