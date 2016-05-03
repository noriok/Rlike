//using UnityEngine;
//using System.Collections;

public abstract class Item {
    public string Name { get; private set; }
    public ItemType Type { get; private set; }

    public Item(ItemType type, string name) {
        Type = type;
        Name = name;
    }

}
