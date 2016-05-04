//using UnityEngine;
using UnityEngine.Assertions;

using System;
using System.Collections;

public class Item {
    public ItemType Type { get; private set; }
    public string Name { get; private set; }
    public int Count { get; private set; } // 石はまとまる
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

    public void Inc(int n) {
        Count += n;
    }

    public void Dec(int n) {
        Count -= n;
        Assert.IsTrue(Count > 0);
    }

    public Item RemoveOne() {
        Dec(1);
        var item = new Item(Type, Name, Desc, Skill);
        return item;
    }

}
