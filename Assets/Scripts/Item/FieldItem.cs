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

    public void UpdatePosition(Vector3 pos) {
        _gobj.transform.position = pos;
    }

    public void UpdateLoc(Loc loc) {
        Loc = loc;
        UpdatePosition(loc.ToPosition());
    }

    private void ChangeSortingLayerName(string sortingLayerName) {
        var renderer = _gobj.GetComponent<SpriteRenderer>();
        renderer.sortingLayerName = sortingLayerName;
    }

    public void BringToFront() {
        ChangeSortingLayerName("Front Base");
    }

    public void ResetZOrder() {
        ChangeSortingLayerName("Item Base");
    }
}

