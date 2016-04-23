using UnityEngine;
using System.Collections;

public static class FieldObjectFactory {
	private static GameObject Create(string path, Vector3 pos, GameObject layer) {
		var gobj = (GameObject)GameObject.Instantiate(Resources.Load(path), pos, Quaternion.identity);
		gobj.transform.SetParent(layer.transform);
		return gobj;
	}

	public static Bonfire CreateBonfire(Loc loc, GameObject layer) {
		var obj = Create("Prefabs/MapObject/bonfire", loc.ToPosition(), layer);
		return new Bonfire(loc, obj);
	}

	public static Treasure CreateTreasure(Loc loc, GameObject layer) {
		var pos = loc.ToPosition();

		var open = Create("Prefabs/MapObject/treasure-open", pos, layer);
		var close = Create("Prefabs/MapObject/treasure-close", pos, layer);
		var anim = Create("Prefabs/Animations/treasure-open-anim", pos, layer);
		return new Treasure(loc, open, close, anim);
	}

	public static NoticeBoard CreateNoticeBoard(Loc loc, GameObject layer, string msg) {
		var obj = Create("Prefabs/MapObject/notice-board", loc.ToPosition(), layer);
		return new NoticeBoard(loc, obj, msg);
	}

	// trap

	public static Trap CreateTrapHeal(Loc loc, GameObject layer) {
		var obj = Create("Prefabs/Trap/trap_heal", loc.ToPosition(), layer);
		return new TrapHeal(loc, obj);
	}

	public static Trap CreateTrapWarp(Loc loc, GameObject layer) {
		var obj = Create("Prefabs/Trap/trap_warp", loc.ToPosition(), layer);
		return new TrapWarp(loc, obj);
	}

	public static Trap CreateTrapDamage(Loc loc, GameObject layer) {
		var obj = Create("Prefabs/Trap/trap_damage", loc.ToPosition(), layer);
		return new TrapDamage(loc, obj);
	}
}
