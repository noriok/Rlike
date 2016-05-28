using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
// using System.Collections;

public class Dialog {
	public bool IsOpen { get; private set; }

	private GameObject _dialog;
	private GameObject _dialogText;

	public Dialog() {
		_dialog = GameObject.Find("Canvas/Dialog");
		_dialogText = GameObject.Find("Canvas/Dialog/Text");

		var btnOK = GameObject.Find("Canvas/Dialog/Button_OK").GetComponent<Button>();

        btnOK.onClick.RemoveAllListeners();
		btnOK.onClick.AddListener(() => {
			Assert.IsTrue(IsOpen);
			IsOpen = false;
			_dialog.SetActive(false);
		});

		_dialog.SetActive(false);
	}

	public void Show(string message) {
		Assert.IsFalse(IsOpen);

		IsOpen = true;
		var text = _dialogText.GetComponent<Text>();
		text.text = message;
		_dialog.SetActive(true);
	}
}
