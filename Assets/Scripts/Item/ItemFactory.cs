using UnityEngine;
using System.Collections;

public static class ItemFactory {

    public static Item CreateGold() {
        var type = ItemType.Gold;
        var name = "お金";
        var desc = "お金だぞ。";
        return new Item(type, name, desc);
    }

    public static Item CreateHerb() {
        var type = ItemType.Herb;
        var name = "薬草";
        var desc = "HP が少しだけ回復するぞ。";
        return new Item(type, name, desc);
    }

    public static Item CreateStone() {
        var type = ItemType.Stone;
        var name = "石";
        var desc = "投げると敵にダメージを与えるぞ。";
        return new Item(type, name, desc);
    }

    public static Item CreateMagic() {
        var type = ItemType.Magic;
        var name = "睡眠の書";
        var desc = "敵を眠らせるぞ。";
        return new Item(type, name, desc);
    }

}
