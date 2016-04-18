using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DamageWait {
	public bool AnimationFinished { get; private set; }

	private CharacterBase _target;

	private IEnumerator Anim() {
		var dmg = 9999;
		_target.DamageHp(dmg);

		yield return EffectAnim.PopupWhiteDigits(_target, dmg);
		if (_target.Hp <= 0) {
			var pos = _target.Position;
			_target.Destroy();
			yield return EffectAnim.Dead(pos);
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

	private IEnumerator MaskFade(GameObject obj, float alphaFrom, float alphaTo) {
		var renderer = obj.GetComponent<SpriteRenderer>();

		float duration = 0.8f;
		float elapsed = 0;
		while (elapsed <= duration) {
			float a = Mathf.Lerp(alphaFrom, alphaTo, elapsed / duration);
			var color = renderer.color;
			color.a = a;
			renderer.color = color;
			elapsed += Time.deltaTime;
			yield return null;
		}
	}

	protected override IEnumerator RunAnimation(MainSystem sys) {
		yield return EffectAnim.Aura(Actor);

		// 黒マスクで画面全体を暗くする
		var mask = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Mask/black"), Actor.Position, Quaternion.identity);
		mask.transform.localScale = new Vector3(10f, 10f, 1);

		yield return MaskFade(mask, 0, 0.5f);

		var obj2 = Resources.Load("Prefabs/Animations/skill");
		var gobj2 = (GameObject)GameObject.Instantiate(obj2, Actor.Position, Quaternion.identity);
		while (gobj2 != null) {
			yield return null;
		}

		yield return MaskFade(mask, 0.5f, 0);
		GameObject.Destroy(mask);

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
	}

	public override void Apply(MainSystem sys) {
		DLog.D("{0} skill", Actor);
	}
}
