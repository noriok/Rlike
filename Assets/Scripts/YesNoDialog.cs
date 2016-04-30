using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
// using System.Collections;

public class YesNoDialog {
	private GameObject _dialog;
	private GameObject _dialogText;

	public bool IsOpen { get; private set; }
	public bool IsYesPressed { get; private set; }
	public bool IsNoPressed { get; private set; }

	public YesNoDialog() {
		_dialog = GameObject.Find("Canvas/YesNoDialog");
		_dialogText = GameObject.Find("Canvas/YesNoDialog/Text");

		var btnYes = GameObject.Find("Canvas/YesNoDialog/Button_Yes").GetComponent<Button>();
		btnYes.onClick.AddListener(() => {
			Assert.IsTrue(IsOpen);
			IsOpen = false;
			IsYesPressed = true;
			_dialog.SetActive(false);
		});

		var btnNo = GameObject.Find("Canvas/YesNoDialog/Button_No").GetComponent<Button>();
		btnNo.onClick.AddListener(() => {
			Assert.IsTrue(IsOpen);
			IsOpen = false;
			IsNoPressed = true;
			_dialog.SetActive(false);
		});

		_dialog.SetActive(false);
	}

	public void Show(string message) {
		Assert.IsFalse(IsOpen);;

		IsYesPressed = IsNoPressed = false;
		IsOpen = true;
		var text = _dialogText.GetComponent<Text>();
		text.text = message;
		_dialog.SetActive(true);
	}
}
