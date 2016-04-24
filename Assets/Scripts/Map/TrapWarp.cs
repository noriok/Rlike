﻿using UnityEngine;
using System.Collections;

public class TrapWarp : Trap {
	public TrapWarp(Loc loc, GameObject gobj) : base(loc, gobj) {

	}

	public override IEnumerator RunAnimation(CharacterBase sender, MainSystem sys) {
		yield return EffectAnim.Warp(sender.Position);

		var src = sender.Position;
		float duration = 0.5f;
		float elapsed = 0;
		while (elapsed <= duration) {
			float y = UTween.Ease(EaseType.InCubic, src.y, src.y + 6, elapsed / duration);
			sender.Position = new Vector3(src.x, y, 0);
			elapsed += Time.deltaTime;
			yield return null;
		}
		yield return new WaitForSeconds(0.36f);

		// TODO:ワープ後の位置
		sender.Position = src;
		sender.ChangeDir(Dir.S);
		yield return new WaitForSeconds(0.4f);
	}

}