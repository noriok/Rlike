using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
// using System.Collections;

public class Button2 : Button {

	public bool Pressed { get; private set; }

	public override void OnPointerDown(PointerEventData eventData) {
		base.OnPointerDown(eventData);
		Debug.Log("OnPointerDown");
		Pressed = true;
	}

	public override void OnPointerUp(PointerEventData eventData) {
		base.OnPointerUp(eventData);
		Debug.Log("OnPointerUp");
		Pressed = false;
	}

	public override void OnPointerEnter(PointerEventData eventData) {
		base.OnPointerEnter(eventData);
		Debug.Log("OnPointerEnter");
		Pressed = true;
	}

	public override void OnPointerExit(PointerEventData eventData) {
		base.OnPointerExit(eventData);
		Debug.Log("OnPointerExit");
		Pressed = false;
	}
}
