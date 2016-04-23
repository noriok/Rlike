using UnityEngine;
using System.Collections;

public abstract class FieldObject {
	public Loc Loc { get; private set; }

	public FieldObject(Loc loc) {
		Loc = loc;
	}

	public virtual IEnumerator RunAnimation() {
		yield break;
	}
}
