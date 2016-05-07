// using UnityEngine;
using UnityEngine.Assertions;

// using System;
using System.Collections;

public class Item {
    public ItemType Type { get { return _data.Type; } }
    public string Name { get { return _data.Name; } }
    public string Desc { get { return _data.Desc; } }
    public int Count { get; private set; }

    private ItemData _data;

    public Item(ItemData data, int count = 1) {
        _data = data;
        Count = count;
    }

    public IEnumerator Use(CharacterBase sender, MainSystem sys) {
        return _data.Skill.Use(sender, sys);
    }

    public IEnumerator Hit(CharacterBase sender, CharacterBase target, MainSystem sys) {
        return _data.Skill.Hit(sender, target, sys);
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
        return new Item(_data);
    }

}
