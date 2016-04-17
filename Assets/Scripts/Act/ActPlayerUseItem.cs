using UnityEngine;
using System.Collections;

public class ActPlayerUseItem : Act {

	public ActPlayerUseItem(Player player) : base(player) {

	}

	private IEnumerator Wait(GameObject obj) {
		while (obj != null) {
			yield return null;
		}

		var healHp = new System.Random().Next(29) + 1;
		var pos = Actor.Position;
		pos.y -= 0.09f;
		yield return EffectAnim.PopupGreenDigits(healHp, pos, () => AnimationFinished = true);
	}

	public override void RunAnimation(MainSystem sys) {
		var obj = Resources.Load("Prefabs/Animations/heal");
		var gobj = (GameObject)GameObject.Instantiate(obj, Actor.Position, Quaternion.identity);
		var pos = gobj.transform.position;
		pos.y += 0.15f;
		gobj.transform.position = pos;
		sys.StartCoroutine(Wait(gobj));
	}

	public override void RunEffect(MainSystem sys) {

	}

}
