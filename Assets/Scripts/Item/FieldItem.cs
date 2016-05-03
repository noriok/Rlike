using UnityEngine;
using System.Collections;

public class FieldItem {
	public Loc Loc { get; private set; }
	private GameObject _gobj;

	public FieldItem(Loc loc, GameObject gobj) {
		Loc = loc;
		_gobj = gobj;
		_gobj.transform.position = loc.ToPosition();
	}

	public void Destroy() {
		GameObject.Destroy(_gobj);
	}
}
