using UnityEngine;

public class NoticeBoard : SimpleFieldObject {
	public string Msg { get; private set; }

	public NoticeBoard(Loc loc, GameObject gobj, string msg) : base(loc, gobj) {
		Msg = msg;
	}
}
