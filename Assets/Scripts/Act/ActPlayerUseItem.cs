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
        sys.Msg_UseItem(_item);
        yield return _item.Use(Actor, sys);
	}

	public override void Apply(MainSystem sys) {
        _player.RemoveItem(_item);
	}
}
