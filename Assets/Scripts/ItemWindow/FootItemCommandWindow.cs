using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class FootItemCommandWindow : MonoBehaviour {
    [SerializeField]
    Image _imageIcon;

    [SerializeField]
    Text _textName;

    [SerializeField]
    Text _textDesc;

    [SerializeField]
    Button _btnClose;

    [SerializeField]
    Button _btnUse;

    [SerializeField]
    Button _btnThrow;

    [SerializeField]
    Button _btnTake;

    Action<ItemActionType, FieldItem> _itemActionCallback;

    private string GetSpritePathName(ItemType itemType) {
        switch (itemType) {
        case ItemType.Herb:
            return "Images/item-herb";
        case ItemType.Magic:
            return "Images/item-book";
        case ItemType.Gold:
            return "Images/item-coin";
        case ItemType.Stone:
            return "Images/item-stone";
        case ItemType.Wand:
            return "Images/item-wand";
        }
        Assert.IsTrue(false);
        return "";
    }

    public void Init(FieldItem fieldItem, Action<ItemActionType, FieldItem> itemActionCallback) {
        _itemActionCallback = itemActionCallback;
        Assert.IsTrue(_itemActionCallback != null);

        Item item = fieldItem.Item;
        _textName.text = item.Name;
        _textDesc.text = item.Desc;

        _imageIcon.sprite = Resources.Load<Sprite>(GetSpritePathName(item.Type));

        if (item.Type == ItemType.Stone) {
            _btnUse.interactable = false;
        }
        else {
            _btnUse.interactable = true;
        }

        _btnClose.onClick.RemoveAllListeners();
        _btnUse.onClick.RemoveAllListeners();
        _btnThrow.onClick.RemoveAllListeners();
        _btnTake.onClick.RemoveAllListeners();

        _btnClose.onClick.AddListener(() => {
            Debug.Log("閉じるが押されました");
            gameObject.SetActive(false);

            _itemActionCallback(ItemActionType.Close, fieldItem);
        });

        _btnUse.onClick.AddListener(() => {
            Debug.Log("使うが押されました");
            gameObject.SetActive(false);

            _itemActionCallback(ItemActionType.Use, fieldItem);
        });

        _btnThrow.onClick.AddListener(() => {
            Debug.Log("投げるが押されました");
            gameObject.SetActive(false);

            _itemActionCallback(ItemActionType.Throw, fieldItem);
        });

        _btnTake.onClick.AddListener(() => {
            Debug.Log("拾うが押されました");
            gameObject.SetActive(false);

            _itemActionCallback(ItemActionType.Take, fieldItem);
        });
    }
}
