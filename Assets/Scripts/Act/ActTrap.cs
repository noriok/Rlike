﻿using UnityEngine;
using System.Collections;

public class ActTrap : Act {

	public ActTrap(CharacterBase target) : base(target) {
	}

	public override bool IsTrapAct() {
		return true;
	}
}
