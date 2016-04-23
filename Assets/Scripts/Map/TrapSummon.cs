using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TrapSummon : Trap {
	public TrapSummon(Loc loc, GameObject gobj) : base(loc, gobj) {

	}

	// TODO:Run rename
	public override IEnumerator RunAnimation(CharacterBase sender, MainSystem sys) {
		var src = sender.Loc;

		// TODO:クロージャが一時変数を共有してしまう問題
		List<Func<IEnumerator>> fns = new List<Func<IEnumerator>>();
		fns.Add(() => sys.Summon(new Loc(src.Row - 1, src.Col)));
		fns.Add(() => sys.Summon(new Loc(src.Row, src.Col + 1)));
		fns.Add(() => sys.Summon(new Loc(src.Row + 1, src.Col + 1)));

		// foreach (var loc in locs) {
		// 	int r = loc.Row;
		// 	int c = loc.Col;
		// 	var l = new Loc(r, c);
		// 	fns.Add(() => sys.Summon(l));
		// }
		yield return Anim.Par(sys, fns.ToArray());
	}
}
