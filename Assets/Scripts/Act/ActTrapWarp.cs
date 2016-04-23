using UnityEngine;
using System.Collections;

public class ActTrapWarp : ActTrap {

	public ActTrapWarp(CharacterBase target) : base(target, null) {
	}

	protected override IEnumerator RunAnimation(MainSystem sys) {
		var src = Actor.Position;
		float duration = 0.5f;
		float elapsed = 0;
		while (elapsed <= duration) {
			float y = Mathf.Lerp(src.y, src.y + 3, elapsed / duration);
			Actor.Position = new Vector3(src.x, y, 0);
			elapsed += Time.deltaTime;
			yield return null;
		}

		// TODO:ワープ後の位置
		Actor.Position = src;
		Actor.ChangeDir(Dir.S);
		yield return new WaitForSeconds(0.4f);
	}

	public override void Apply(MainSystem sys) {
	}
}
