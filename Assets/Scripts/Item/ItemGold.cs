using UnityEngine;
using System.Collections;

public class ItemGold : Item {
	public int Value { get; private set; }

	public ItemGold(int value) : base(ItemType.Gold, "お金") {
		Value = value;
	}

}
