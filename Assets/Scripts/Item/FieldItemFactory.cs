using UnityEngine;
using System.Collections;

public static class FieldItemFactory {

    private static FieldItem Create(string path, Item item, Loc loc, GameObject layer) {
        var obj = Resources.Load(path);
        var gobj = (GameObject)GameObject.Instantiate(obj);
        gobj.transform.SetParent(layer.transform);
        return new FieldItem(item, loc, gobj);
    }

    public static FieldItem CreateGold(Loc loc, GameObject layer) {
        var item = ItemFactory.CreateGold();
        return Create("Prefabs/Item/item-coin", item, loc, layer);
    }

    public static FieldItem CreateHerb(Loc loc, GameObject layer) {
        var item = ItemFactory.CreateHerb();
        return Create("Prefabs/Item/item-herb", item, loc, layer);
    }
}
