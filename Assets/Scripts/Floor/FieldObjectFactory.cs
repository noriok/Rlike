using UnityEngine;
// using System.Collections;

public static class FieldObjectFactory {
	private static GameObject Create(string path, Vector3 pos) {
        var layer = LayerManager.GetLayer(LayerName.Trap);

		var gobj = (GameObject)GameObject.Instantiate(Resources.Load(path), pos, Quaternion.identity);
		gobj.transform.SetParent(layer.transform);
		return gobj;
	}

	public static Bonfire CreateBonfire(Loc loc, GameObject layer) {
		var obj = Create("Prefabs/MapObject/bonfire", loc.ToPosition());
		return new Bonfire(loc, obj);
	}

	public static Treasure CreateTreasure(Loc loc, GameObject layer) {
		var pos = loc.ToPosition();

		var open = Create("Prefabs/MapObject/treasure-open", pos);
		var close = Create("Prefabs/MapObject/treasure-close", pos);
		var anim = Create("Prefabs/Animations/treasure-open-anim", pos);
		return new Treasure(loc, open, close, anim);
	}

	public static NoticeBoard CreateNoticeBoard(Loc loc, string msg) {
		var obj = Create("Prefabs/MapObject/notice-board", loc.ToPosition());
		return new NoticeBoard(loc, obj, msg);
	}

	public static void CreateStairs(Loc loc) {
		Create("Prefabs/MapChip/mapchip-stairs", loc.ToPosition());
	}

	// trap

	public static Trap CreateTrapHeal(Loc loc) {
		var obj = Create("Prefabs/Trap/trap_heal", loc.ToPosition());
		return new TrapHeal(loc, obj);
	}

	public static Trap CreateTrapWarp(Loc loc) {
		var obj = Create("Prefabs/Trap/trap_warp", loc.ToPosition());
		return new TrapWarp(loc, obj);
	}

	public static Trap CreateTrapDamage(Loc loc) {
		var obj = Create("Prefabs/Trap/trap_damage", loc.ToPosition());
		return new TrapDamage(loc, obj);
	}

	public static Trap CreateTrapSummon(Loc loc) {
		var obj = Create("Prefabs/Trap/trap_summon", loc.ToPosition());
		return new TrapSummon(loc, obj);
	}

    public static Trap CreateTrapLandmine(Loc loc) {
		var obj = Create("Prefabs/Trap/trap_landmine", loc.ToPosition());
		return new TrapLandmine(loc, obj);
    }
}
