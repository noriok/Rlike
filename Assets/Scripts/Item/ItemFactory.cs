using UnityEngine;
using System.Collections;

public static class ItemFactory {
	public static FieldItem CreateFieldItem(Loc loc, GameObject layer) {
		var obj = Resources.Load("Prefabs/Item/item-book");
		var gobj = (GameObject)GameObject.Instantiate(obj);
		gobj.transform.SetParent(layer.transform);
		return new FieldItem(loc, gobj);
	}
}
