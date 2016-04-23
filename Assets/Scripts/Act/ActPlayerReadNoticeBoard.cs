using UnityEngine;
using System.Collections;

public class ActPlayerReadNoticeBoard : Act {
	private NoticeBoard _noticeBoard;

	public ActPlayerReadNoticeBoard(Player player, NoticeBoard noticeBoard) : base(player) {
		_noticeBoard = noticeBoard;
	}

	public override void Apply(MainSystem sys) {
		Debug.Log("立て札: " + _noticeBoard.Msg);
		DLog.D("{0} notice board", Actor);
	}
}
