using UnityEngine;
// using System.Collections;

public class ItemData {
    public ItemType Type { get; private set; }
    public string Name { get; private set; }
    public string Desc { get; private set; }
    public Skill Skill { get; private set; }

    public ItemData(ItemType type, string name, string desc, Skill skill) {
        Type = type;
        Name = name;
        Desc = desc;
        Skill = skill;
    }
}
