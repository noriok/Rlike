using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System;
using System.Collections;

public class ItemCommandWindow : MonoBehaviour {
    [SerializeField]
    GameObject _itemWindow;

    [SerializeField]
    Image _imageIcon;

    [SerializeField]
    Text _textName;

    [SerializeField]
    Text _textDesc;

    [SerializeField]
    Button _btnBack;

    [SerializeField]
    Button _btnUse;

    [SerializeField]
    Button _btnThrow;

    [SerializeField]
    Button _btnPut;

    Action<ItemActionType, Item> _itemActionCallback;

	// Use this for initialization
    void Start () {
    }

	// Update is called once per frame
    void Update () {
    }

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
        }
        Assert.IsTrue(false);
        return "";
    }

    public void Init(Item item, Action<ItemActionType, Item> itemActionCallback) {
        _itemActionCallback = itemActionCallback;
        Assert.IsTrue(_itemActionCallback != null);

        _textName.text = item.Name;
        _textDesc.text = item.Desc;

        _imageIcon.sprite = Resources.Load<Sprite>(GetSpritePathName(item.Type));

        if (item.Type == ItemType.Stone) {
            _btnUse.interactable = false;
        }
        else {
            _btnUse.interactable = true;
        }

        _btnBack.onClick.RemoveAllListeners();
        _btnUse.onClick.RemoveAllListeners();
        _btnThrow.onClick.RemoveAllListeners();
        _btnPut.onClick.RemoveAllListeners();

        _btnBack.onClick.AddListener(() => {
            Debug.Log("戻るが押されました");
            gameObject.SetActive(false);
            _itemWindow.SetActive(true);
        });

        _btnUse.onClick.AddListener(() => {
            Debug.Log("使うが押されました");
            gameObject.SetActive(false);

            _itemActionCallback(ItemActionType.Use, item);
        });

        _btnThrow.onClick.AddListener(() => {
            Debug.Log("投げるが押されました");
            gameObject.SetActive(false);

            _itemActionCallback(ItemActionType.Throw, item);
        });

        _btnPut.onClick.AddListener(() => {
            Debug.Log("置くが押されました");
            gameObject.SetActive(false);

            _itemActionCallback(ItemActionType.Put, item);
        });
    }
}
