using UnityEngine;
using UnityEngine.Assertions;
using System;
using UnityEngine.UI;

public class YesNoDialog {

    public YesNoDialog(GameObject dialog, string message, Action yes, Action no) {
        dialog.SetActive(true);
        var text = dialog.transform.Find("Text").GetComponent<Text>();
        text.text = message;

        var btnYes = dialog.transform.Find("Button_Yes").GetComponent<Button>();
        btnYes.onClick.RemoveAllListeners();
        btnYes.onClick.AddListener(() => {
            dialog.SetActive(false);
            yes();
        });

        var btnNo = dialog.transform.Find("Button_No").GetComponent<Button>();
        btnNo.onClick.RemoveAllListeners();
        btnNo.onClick.AddListener(() => {
            dialog.SetActive(false);
            no();
        });
    }
}
