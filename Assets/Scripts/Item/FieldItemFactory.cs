using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public static class FieldItemFactory {

    private static FieldItem Create(Item item, Loc loc, GameObject layer) {
        var obj = Resources.Load(GetPrefabPathName(item.Type));
        var gobj = (GameObject)GameObject.Instantiate(obj);
        gobj.transform.SetParent(layer.transform);
        return new FieldItem(item, loc, gobj);
    }

    private static string GetPrefabPathName(ItemType itemType) {
        switch (itemType) {
        case ItemType.Gold:  return "Prefabs/Item/item-coin";
        case ItemType.Herb:  return "Prefabs/Item/item-herb";
        case ItemType.Magic: return "Prefabs/Item/item-book";
        case ItemType.Stone: return "Prefabs/Item/item-stone";
        case ItemType.Wand:  return "Prefabs/Item/item-wand";
        }

        Assert.IsTrue(false);
        return "";
    }

    public static FieldItem CreateGold(Loc loc, GameObject layer) {
        var item = ItemFactory.CreateGold();
        return Create(item, loc, layer);
    }

    public static FieldItem CreateHerb(Loc loc, GameObject layer) {
        var item = ItemFactory.CreateHerb();
        return Create(item, loc, layer);
    }

    public static FieldItem CreateWand(Loc loc, GameObject layer) {
        var item = ItemFactory.CreateWand();
        return Create(item, loc, layer);
    }

    public static FieldItem CreateMagic(Loc loc, GameObject layer) {
        var item = ItemFactory.CreateMagic();
        return Create(item, loc, layer);
    }

    public static FieldItem CreateFromItem(Item item, Loc loc) {
        // TODO:layer
        var layer = GameObject.Find(LayerName.Item);
        return Create(item, loc, layer);
    }
}
