using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TrapLandmine : Trap {

    public TrapLandmine(Loc loc, GameObject gobj) : base(loc, gobj) {

    }

    public override IEnumerator RunAnimation(CharacterBase sender, MainSystem sys) {
        yield return EffectAnim.Landmine(sender.Position);

        var enemies = sys.CollectNeighborEnemies(sender.Loc);
        var damageAnims = new List<DamageWait>();
        damageAnims.Add(new DamageWait(sender, Math.Max(1, sender.Hp - 1), sys));
        for (int i = 0; i < enemies.Length; i++) {
            damageAnims.Add(new DamageWait(enemies[i], 99, sys));
        }

        while (true) {
            bool finished = true;
            foreach (var d in damageAnims) {
                finished = finished && d.AnimationFinished;
            }

            if (finished) break;
            yield return null;
        }
    }

    public override string Name() {
        return "地雷のワナ";
    }

    public override string Description() {
        return "踏むと周囲にダメージを与えるぞ。自分もダメージを受けるぞ";
    }

    public override string ImagePath() {
        return "Images/trap4";
    }
}
