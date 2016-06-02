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
        sys.Message(string.Format("{0} を使った", _item.Name));
        yield return _item.Use(Actor, sys);
	}

	public override void Apply(MainSystem sys) {
        _player.RemoveItem(_item);
	}
}
