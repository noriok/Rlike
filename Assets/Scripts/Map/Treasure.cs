using UnityEngine;
using System.Collections;

public class Treasure : FieldObject {
	private GameObject _open;
	private GameObject _close;
	private GameObject _anim;

	public Treasure(Loc loc, GameObject open, GameObject close, GameObject anim) : base(loc, null) { // TODO:宝箱作り直し
		_open = open;
		_close = close;
		_anim = anim;

		_open.SetActive(false);
		_anim.SetActive(false);
	}

	public override IEnumerator RunAnimation(CharacterBase sender, MainSystem sys) {
		_open.SetActive(false);
		_close.SetActive(false);
		_anim.SetActive(true);

		var animator = _anim.GetComponent<Animator>();
		while (true) {
			var animInfo = animator.GetCurrentAnimatorStateInfo(0);
			if (animInfo.normalizedTime >= 1.0f) {
				break;
			}
			yield return null;
		}
		yield return new WaitForSeconds(0.2f);

		_anim.SetActive(false);
		_open.SetActive(true);
	}
}
