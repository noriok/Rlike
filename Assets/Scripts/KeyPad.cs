using UnityEngine;
// using System.Collections;

public class KeyPad {
	private Button2 _btnN;
	private Button2 _btnNE;
	private Button2 _btnE;
	private Button2 _btnSE;
	private Button2 _btnS;
	private Button2 _btnSW;
	private Button2 _btnW;
	private Button2 _btnNW;

	private Button2 _btnAction;

	public KeyPad() {
		_btnN = GameObject.Find("Canvas/Button_N").GetComponent<Button2>();
		_btnNE = GameObject.Find("Canvas/Button_NE").GetComponent<Button2>();
		_btnE = GameObject.Find("Canvas/Button_E").GetComponent<Button2>();
		_btnSE = GameObject.Find("Canvas/Button_SE").GetComponent<Button2>();
		_btnS = GameObject.Find("Canvas/Button_S").GetComponent<Button2>();
		_btnSW = GameObject.Find("Canvas/Button_SW").GetComponent<Button2>();
		_btnW = GameObject.Find("Canvas/Button_W").GetComponent<Button2>();
		_btnNW = GameObject.Find("Canvas/Button_NW").GetComponent<Button2>();

		_btnAction = GameObject.Find("Canvas/Button_Action").GetComponent<Button2>();
	}

	public bool IsAttack() {
		if (Input.GetKey(KeyCode.Space)) return true;

		if (_btnAction.Pressed) return true;
		// if (Input.touchCount > 0) {
		// 	Touch touch = Input.GetTouch(0);
		// 	if (touch.phase == TouchPhase.Ended) {
		// 		return true;
		// 	}
		// }
		return false;
	}

	public bool IsMove(out Dir dir) {
		bool buttonPressed = true;
		dir = Dir.N;
		if (_btnN.Pressed) dir = Dir.N;
		else if (_btnNE.Pressed) dir = Dir.NE;
		else if (_btnE.Pressed) dir = Dir.E;
		else if (_btnSE.Pressed) dir = Dir.SE;
		else if (_btnS.Pressed) dir = Dir.S;
		else if (_btnSW.Pressed) dir = Dir.SW;
		else if (_btnW.Pressed) dir = Dir.W;
		else if (_btnNW.Pressed) dir = Dir.NW;
		else buttonPressed = false;

		if (buttonPressed) return true;

		bool keyPressed = true;
        if (Input.GetKey(KeyCode.J)) { // 南
			dir = Dir.S;
        }
        else if (Input.GetKey(KeyCode.K)) { // 北
			dir = Dir.N;
        }
        else if (Input.GetKey(KeyCode.L)) { // 東
			dir = Dir.E;
        }
        else if (Input.GetKey(KeyCode.H)) { // 西
			dir = Dir.W;
        }
        else if (Input.GetKey(KeyCode.B)) { // 南西
			dir = Dir.SW;
        }
        else if (Input.GetKey(KeyCode.N)) { // 南東
			dir = Dir.SE;
        }
        else if (Input.GetKey(KeyCode.Y)) { // 北西
			dir = Dir.NW;
        }
        else if (Input.GetKey(KeyCode.U)) { // 北東
			dir = Dir.NE;
        }
		else keyPressed = false;

		return keyPressed;
	}



}
