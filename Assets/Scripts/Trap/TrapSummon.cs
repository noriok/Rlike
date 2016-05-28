using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TrapSummon : Trap {
	public TrapSummon(Loc loc, GameObject gobj) : base(loc, gobj) {
	}

	// 一時変数を共有するクロージャとイテレータを同時に使うのを避けるため関数化する
	// TODO:
	// 周囲八マスからランダムに選ぶ
	// その位置に敵がいないことを確認
	private Func<IEnumerator>[] Summon(Loc src, MainSystem sys) {
        var xs = new List<Loc>(src.Neighbors());
        Utils.Shuffle(xs);

        var rand = new System.Random();
        int n = rand.Next(2, 3);
        var locs = new List<Loc>();
        for (int i = 0; i < xs.Count; i++) {
            // TODO: 敵の配置だけでなく、敵が配置可能かも調べる
            if (!sys.ExistsEnemy(xs[i])) {
                locs.Add(xs[i]);

                if (locs.Count == n) break;
            }
        }

		List<Func<IEnumerator>> fns = new List<Func<IEnumerator>>();
		for (int i = 0; i < locs.Count; i++) {
			int p = i;
			fns.Add(() => sys.Summon(locs[p]));
		}
		return fns.ToArray();
	}

	// TODO:Run rename
	public override IEnumerator RunAnimation(CharacterBase sender, MainSystem sys) {
        var xs = Summon(sender.Loc, sys);

        if (xs.Length == 0) {
            yield break;
        }
        else {
            yield return Anim.Par(sys, xs);
		    // yield return Anim.Par(sys, Summon(sender.Loc, sys));
        }
	}

    public override string Name() {
        return "召喚のワナ";
    }

    public override string Description() {
        return "踏むと周囲にモンスターを召喚するぞ。";
    }

    public override string ImagePath() {
        return "Images/trap0";
    }
}
