﻿using UnityEngine;
using UnityEngine.UI;
// using System.Collections;

public class MessageManager {
    private GameObject _text1;
    private GameObject _text2;
    // private GameObject _text3;
    private MainSystem _sys;

    private float _lastUpdateTime;

    public MessageManager(MainSystem sys) {
        _text1 = GameObject.Find("Canvas/MessageWindow/Panel/Text1");
        _text2 = GameObject.Find("Canvas/MessageWindow/Panel/Text2");
        // _text3 = GameObject.Find("Canvas/MessageWindow/Panel/Text3");
        Clear();
    }

    public void Update() {
        if (_lastUpdateTime > 0) {
            float d = Time.time - _lastUpdateTime;
            if (d >= 2) {
                Clear();
            }
        }
    }

    public void Clear() {
        _text1.GetComponent<Text>().text = "";
        _text2.GetComponent<Text>().text = "";
        _lastUpdateTime = 0;
    }

    public void Message(string msg) {
        _text1.GetComponent<Text>().text = msg;
        _text2.GetComponent<Text>().text = "";
        _lastUpdateTime = Time.time;
    }

    public void Message(string msg1, string msg2) {
        _text1.GetComponent<Text>().text = msg1;
        _text2.GetComponent<Text>().text = msg2;
        _lastUpdateTime = Time.time;
    }

    public void UseItem(Item item) {
        string doing = "使った！";
        switch (item.Type) {
        default:
        case ItemType.Herb:
        case ItemType.Gold:
            doing = "使った！";
            break;
        case ItemType.Magic:
            doing = "読んだ！";
            break;
        case ItemType.Wand:
            doing = "振った！";
            break;
        }
        Message(string.Format("{0} を{1}", item.Name, doing));
    }

    public void ThrowItem(Item item) {
        Message(string.Format("{0} を{1}", item.Name, "投げた！"));
    }

    public void TakeItem(Item item) {
        Message(string.Format("{0} を{1}", item.Name, "ひろった"));
    }
}
