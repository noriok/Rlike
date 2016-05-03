using UnityEngine;
using System.Collections;

public class FieldItem {
	public Item Item { get; private set; }
	public Loc Loc { get; private set; }
	private GameObject _gobj;

	public FieldItem(Item item, Loc loc, GameObject gobj) {
		Item = item;
		Loc = loc;
		_gobj = gobj;
		_gobj.transform.position = loc.ToPosition();
	}

	public void Destroy() {
		GameObject.Destroy(_gobj);
	}
}
