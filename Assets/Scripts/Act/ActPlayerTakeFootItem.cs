using UnityEngine;

public class ActPlayerTakeFootItem : Act {
    private FieldItem _fieldItem;

    public ActPlayerTakeFootItem(Player player, FieldItem fieldItem) : base(player) {
        _fieldItem = fieldItem;
    }

    public override void OnFinished(MainSystem sys) {
        Debug.LogFormat("アイテムを拾いました:{0}", _fieldItem);

        // TODO:ActPlayerMoveのアイテムを拾う処理と重複
        Item item = _fieldItem.Item;
        if (item.Type == ItemType.Gold) {
            Debug.LogFormat("{0} G 手に入れた", 100);
            sys.IncGold(100);
        }
        else {
            // TODO:持ち物がいっぱいなら拾えない
            ((Player)Actor).AddItem(item);
        }
        sys.RemoveFieldItem(_fieldItem);
    }
}
