using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DamageWait {
	public bool AnimationFinished { get; private set; }

	private CharacterBase _target;

	private IEnumerator Anim() {
		var dmg = 9999;
		_target.DamageHp(dmg);

		var pos = _target.Position;
		pos.y -= 0.09f;
		yield return EffectAnim.PopupWhiteDigits(dmg, pos, () => {});
		if (_target.Hp <= 0) {
			var obj = Resources.Load("Prefabs/Animations/dead");
			var gobj = (GameObject)GameObject.Instantiate(obj, _target.Position, Quaternion.identity);
			_target.Destroy();
			while (gobj != null) {
				yield return null;
			}
		}
		AnimationFinished = true;
	}

	public DamageWait(CharacterBase target, MainSystem sys) {
		_target = target;
		sys.StartCoroutine(Anim());
	}
}

public class ActPlayerUseSkill : Act {
	private CharacterBase[] _targets;

	public ActPlayerUseSkill(Player player, CharacterBase[] targets) : base(player) {
		_targets = targets;
	}

	private IEnumerator Anim(MainSystem sys) {
		var obj = Resources.Load("Prefabs/Animations/aura");
		var gobj = (GameObject)GameObject.Instantiate(obj, Actor.Position, Quaternion.identity);
		while (gobj != null) {
			yield return null;
		}

		var obj2 = Resources.Load("Prefabs/Animations/skill");
		var gobj2 = (GameObject)GameObject.Instantiate(obj2, Actor.Position, Quaternion.identity);
		while (gobj2 != null) {
			yield return null;
		}

		var damageAnims = new List<DamageWait>();
		for (int i = 0; i < _targets.Length; i++) {
			damageAnims.Add(new DamageWait(_targets[i], sys));
		}

		while (true) {
			bool finished = true;
			foreach (var d in damageAnims) {
				finished = finished && d.AnimationFinished;
			}

			if (finished) break;
			yield return null;
		}

		AnimationFinished = true;
	}

	public override void RunAnimation(MainSystem sys) {
		sys.StartCoroutine(Anim(sys));
	}

	public override void RunEffect(MainSystem sys) {
		DLog.D("{0} skill", Actor);
	}
}
