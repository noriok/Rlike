using UnityEngine;
using UnityEngine.Assertions;
using System;
using UnityEngine.UI;

public class YesNoDialog {

	public YesNoDialog(GameObject dialog, string message, Action yesCallback, Action noCallback) {
        dialog.SetActive(true);
        var text = dialog.transform.Find("Text").GetComponent<Text>();
        text.text = message;

		var btnYes = GameObject.Find("Canvas/YesNoDialog/Button_Yes").GetComponent<Button>();
        btnYes.onClick.RemoveAllListeners();
		btnYes.onClick.AddListener(() => {
            dialog.SetActive(false);
            yesCallback();
		});

		var btnNo = GameObject.Find("Canvas/YesNoDialog/Button_No").GetComponent<Button>();
        btnNo.onClick.RemoveAllListeners();
		btnNo.onClick.AddListener(() => {
            dialog.SetActive(false);
            noCallback();
		});
	}
}
