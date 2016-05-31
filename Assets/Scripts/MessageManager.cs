using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MessageManager {
    private GameObject _text1;
    private GameObject _text2;
    private GameObject _text3;
    private MainSystem _sys;

    private Time _lastUpdateTime;

    public MessageManager(MainSystem sys) {
        _text1 = GameObject.Find("Canvas/MessageWindow/Panel/Text1");
        _text2 = GameObject.Find("Canvas/MessageWindow/Panel/Text2");
        _text3 = GameObject.Find("Canvas/MessageWindow/Panel/Text3");

        // var text = _text1.GetComponent<Text>();
        // text.text = "";


    }

    public void Update() {
    }
}
