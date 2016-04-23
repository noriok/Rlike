using UnityEngine;
using System.Collections;

public class SimpleFieldObject : FieldObject {
	private GameObject _gobj;

	public SimpleFieldObject(Loc loc, GameObject gobj) : base(loc) {
		_gobj = gobj;
	}
}
