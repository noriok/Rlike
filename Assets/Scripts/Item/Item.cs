//using UnityEngine;
//using System.Collections;

public class Item {
    public ItemType Type { get; private set; }
    public string Name { get; private set; }
    public int Count { get; private set; }
    public string Desc { get { return "アイテムの説明"; } }

    public Item(ItemType type, string name, int count) {
        Type = type;
        Name = name;
        Count = count;
    }
}
