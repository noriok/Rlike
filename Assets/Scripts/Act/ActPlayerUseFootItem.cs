﻿using UnityEngine;
using System.Collections;

public class ActPlayerUseFootItem : Act {
    private FieldItem _fieldItem;

	public ActPlayerUseFootItem(Player player, FieldItem fieldItem) : base(player) {
        _fieldItem = fieldItem;
	}

	protected override IEnumerator RunAnimation(MainSystem sys) {
        Item item = _fieldItem.Item;
        sys.RemoveFieldItem(_fieldItem);
        yield return item.Use(Actor, sys);
	}

	public override void Apply(MainSystem sys) {
		DLog.D("{0} item", Actor);
	}
}