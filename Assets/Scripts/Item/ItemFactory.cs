using UnityEngine;
using System.Collections;

public static class ItemFactory {
    // public static FieldItem CreateFieldItem(Loc loc, GameObject layer) {
    // 	var obj = Resources.Load("Prefabs/Item/item-book");
    // 	var gobj = (GameObject)GameObject.Instantiate(obj);
    // 	gobj.transform.SetParent(layer.transform);
    // 	return new FieldItem(loc, gobj);
    // }

    private static FieldItem Create(string path, Item item, Loc loc, GameObject layer) {
        var obj = Resources.Load(path);
        var gobj = (GameObject)GameObject.Instantiate(obj);
        gobj.transform.SetParent(layer.transform);
        return new FieldItem(item, loc, gobj);
    }

    public static FieldItem CreateGold(Loc loc, GameObject layer) {
        var item = new ItemGold(100);
        return Create("Prefabs/Item/item-coin", item, loc, layer);
    }


}
