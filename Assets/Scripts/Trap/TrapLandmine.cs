using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrapLandmine : Trap {

    public TrapLandmine(Loc loc, GameObject gobj) : base(loc, gobj) {

    }

    public override IEnumerator RunAnimation(CharacterBase sender, MainSystem sys) {
        yield return EffectAnim.Landmine(sender.Position);

        var enemies = sys.CollectNeighborEnemies(sender.Loc);
        if (enemies.Length > 0) {
            var damageAnims = new List<DamageWait>();
            for (int i = 0; i < enemies.Length; i++) {
                damageAnims.Add(new DamageWait(enemies[i], sys));
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
    }
}
