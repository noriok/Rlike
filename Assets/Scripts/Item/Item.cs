//using UnityEngine;
//using System.Collections;

public class Item {
    public ItemType Type { get; private set; }
    public string Name { get; private set; }
    public int Count { get; private set; }
    public string Desc { get; private set; }

    public Item(ItemType type, string name, string desc, int count = 1) {
        Type = type;
        Name = name;
        Desc = desc;
        Count = count;
    }
}
