using UnityEngine;
using System.Collections;

public class ActPlayerUseSkill : Act {
	private CharacterBase[] _targets;

	public ActPlayerUseSkill(Player player, CharacterBase[] targets) : base(player) {
		_targets = targets;

	}

	// TODO:全てのアニメを同時に実行する
	private IEnumerator DamageAnim() {
		for (int i = 0; i < _targets.Length; i++) {
			var dmg = 9999;
			_targets[i].DamageHp(dmg);

			var pos = _targets[i].Position;
			pos.y -= 0.09f;
			yield return EffectAnim.PopupWhiteDigits(dmg, pos, () => {});
			if (_targets[i].Hp <= 0) {
				var obj = Resources.Load("Prefabs/Animations/dead");
				var gobj = (GameObject)GameObject.Instantiate(obj, _targets[i].Position, Quaternion.identity);
				_targets[i].Destroy();
				while (gobj != null) {
					yield return null;
				}
			}
		}
	}

	private IEnumerator Anim() {
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

		yield return DamageAnim();

		AnimationFinished = true;
	}

	public override void RunAnimation(MainSystem sys) {
		sys.StartCoroutine(Anim());
	}

	public override void RunEffect(MainSystem sys) {
		DLog.D("{0} skill", Actor);
	}
}
