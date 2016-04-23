using UnityEngine;
using System.Collections;

public class ActTrapSummonEnemy : ActTrap {

	public ActTrapSummonEnemy(CharacterBase target) : base(target) {

	}

	protected override IEnumerator RunAnimation(MainSystem sys) {
		yield break;
	}

	public override void Apply(MainSystem sys) {

	}

}
