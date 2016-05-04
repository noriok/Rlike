using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System;
using System.Collections;
using System.Collections.Generic;

public class ScrollController : MonoBehaviour {
    [SerializeField]
    RectTransform _prefab;

    [SerializeField]
    GameObject _itemCommandWindow;

    [SerializeField]
    Button _btnClose;

    Action _closeCallback;

    Action<ItemActionType, Item> _itemActionCallback;

    void Start () {
        _itemCommandWindow.SetActive(false);

        _btnClose.onClick.AddListener(() => {
            GameObject.Find("Canvas/ScrollView").SetActive(false);
            if (_closeCallback != null) {
                _closeCallback();
            }
        });
    }

    void Update () {
    }

    public void SetCloseCallback(Action closeCallback) {
        _closeCallback = closeCallback;
    }

    public void SetItemActionCallback(Action<ItemActionType, Item> callback) {
        _itemActionCallback = callback;
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

    public void Init(List<Item> items) {
        // 子オブジェクトを削除
        var content = GameObject.Find("Canvas/ScrollView/Panel/Content");
        foreach (Transform a in content.transform) {
            Destroy(a.gameObject);
        }

        foreach (var item in items) {
            Add(item);
        }
    }

    public void Add(Item item) {
        var node = GameObject.Instantiate(_prefab) as RectTransform;
	    node.SetParent(transform, false);

        var text = node.GetComponentInChildren<Text>();
        if (item.Count == 1) {
            text.text = item.Name;
        }
        else {
            text.text = string.Format("{0} x{1}", item.Name, item.Count);
        }

        var icon = node.FindChild("Icon").GetComponent<Image>();
        icon.sprite = Resources.Load<Sprite>(GetSpritePathName(item.Type));

        var btn = node.FindChild("Button").GetComponent<Button>();
        btn.onClick.AddListener(() => {
            Debug.Log("選択されたアイテム = " + item.Name);
            GameObject.Find("Canvas/ScrollView").SetActive(false);

            var win = _itemCommandWindow.GetComponent<ItemCommandWindow>();
            win.Init(item, _itemActionCallback);
            _itemCommandWindow.SetActive(true);
        });
    }
}
