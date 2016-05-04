using UnityEngine;
using System.Collections;

public class ActPlayerPutItem : Act {
    private Player _player;
    private Item _item;

    public ActPlayerPutItem(Player player, Item item) : base(player) {
        _player = player;
        _item = item;
    }

    public override void Apply(MainSystem sys) {
        Debug.Log("アイテムを置きました");

        var item = _player.RemoveItem(_item);
        sys.AddFieldItem(FieldItemFactory.CreateFromItem(item, _player.Loc));
    }
}
