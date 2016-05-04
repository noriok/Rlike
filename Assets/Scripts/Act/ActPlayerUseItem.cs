using UnityEngine;
// using UnityEngine.Assertions;
using System.Collections;

public class ActPlayerUseItem : Act {
    private Player _player;
    private Item _item;

	public ActPlayerUseItem(Player player, Item item) : base(player) {
        _player = player;
        _item = item;
	}

	protected override IEnumerator RunAnimation(MainSystem sys) {
        yield return _item.Use(Actor, sys);
	}

	public override void Apply(MainSystem sys) {
		DLog.D("{0} item", Actor);

        Debug.LogFormat("item:{0} を使いました", _item);
        _player.RemoveItem(_item);
	}
}
