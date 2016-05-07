using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DamageWait {
	public bool AnimationFinished { get; private set; }

	private CharacterBase _target;

	private IEnumerator Run(MainSystem sys) {
		var dmg = 999;

		_target.RemoveStatus(Status.Sleep);
		yield return Anim.Par(sys,
							  () => _target.DamageAnim(dmg),
							  () => EffectAnim.PopupWhiteDigits(_target, dmg));
		_target.DamageHp(dmg);
		if (_target.Hp <= 0) {
			var pos = _target.Position;
			_target.Destroy();
			yield return EffectAnim.Dead(pos);
		}
		AnimationFinished = true;
	}

	public DamageWait(CharacterBase target, MainSystem sys) {
		_target = target;
		sys.StartCoroutine(Run(sys));
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
		yield return EffectAnim.Aura(Actor.Position);

		sys.HideMinimap();

		// 黒マスクで画面全体を暗くする
		var mask = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Mask/black"), Actor.Position, Quaternion.identity);
		mask.transform.localScale = new Vector3(10f, 15f, 1);

		float cameraZoomDelta = 0.65f;
		yield return Anim.Par(sys,
			                  () => sys.CameraZoomOut(cameraZoomDelta),
							  () => MaskFade(mask, 0, 0.5f));

		yield return Anim.Par(sys,
		                      () => CAction.Quake(GameObject.Find(LayerName.Map), 1.4f),
							  () => EffectAnim.Skill(Actor.Position));
		yield return new WaitForSeconds(0.3f);

		yield return Anim.Par(sys,
		                      () => sys.CameraZoomIn(cameraZoomDelta),
							  () => MaskFade(mask, 0.5f, 0));
		GameObject.Destroy(mask);

		sys.ShowMinimap();

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
