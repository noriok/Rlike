//using UnityEngine;

using System;
using System.Collections;

public class Item {
    public ItemType Type { get; private set; }
    public string Name { get; private set; }
    public int Count { get; private set; }
    public string Desc { get; private set; }
    public Skill Skill { get; private set; }

    public Item(ItemType type, string name, string desc, Skill skill) {
        Type = type;
        Name = name;
        Desc = desc;
        Count = 1;
        Skill = skill;
    }

    public IEnumerator Use(CharacterBase sender, MainSystem sys) {
        return Skill.Use(sender, sys);
    }


}
