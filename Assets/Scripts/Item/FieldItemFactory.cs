using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public static class FieldItemFactory {

    private static FieldItem Create(Item item, Loc loc) {
        var layer = LayerManager.GetLayer(LayerName.Item);

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
        return Create(item, loc);
    }

    public static FieldItem CreateHerb(Loc loc, int index) {
        var item = ItemFactory.CreateHerb(index);
        return Create(item, loc);
    }

    public static FieldItem CreateWand(Loc loc, int index) {
        var item = ItemFactory.CreateWand(index);
        return Create(item, loc);
    }

    public static FieldItem CreateMagic(Loc loc, int index) {
        var item = ItemFactory.CreateMagic(index);
        return Create(item, loc);
    }

    public static FieldItem CreateFromItem(Item item, Loc loc) {
        // TODO:layer
        var layer = GameObject.Find(LayerName.Item);
        return Create(item, loc);
    }
}
