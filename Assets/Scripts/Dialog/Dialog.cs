using System;
using UnityEngine;
using UnityEngine.UI;

// TODO: rename OKDialog
public class Dialog {

    public Dialog(GameObject dialog, string message, Action ok) {
        dialog.SetActive(true);
        var text = dialog.transform.Find("Text").GetComponent<Text>();
        text.text = message;

        var btnOK = dialog.transform.Find("Button_OK").GetComponent<Button>();
        btnOK.onClick.RemoveAllListeners();
        btnOK.onClick.AddListener(() => {
            dialog.SetActive(false);
            ok();
        });
    }
}
