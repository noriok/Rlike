using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TrapSummon : Trap {
	public TrapSummon(Loc loc, GameObject gobj) : base(loc, gobj) {

	}

	// TODO:
	// 周囲八マスからランダムに選ぶ
	// その位置に敵がいないことを確認
	private Func<IEnumerator>[] Foo(Loc src, MainSystem sys) {
		var locs = new List<Loc>();
		locs.Add(new Loc(src.Row - 1, src.Col));
		locs.Add(new Loc(src.Row, src.Col + 1));
		locs.Add(new Loc(src.Row + 1, src.Col + 1));

		List<Func<IEnumerator>> fns = new List<Func<IEnumerator>>();
		for (int i = 0; i < locs.Count; i++) {
			int p = i;
			fns.Add(() => sys.Summon(locs[p]));
		}
		return fns.ToArray();
	}

	// TODO:Run rename
	public override IEnumerator RunAnimation(CharacterBase sender, MainSystem sys) {
		var src = sender.Loc;
		yield return Anim.Par(sys, Foo(src, sys));
	}
}
