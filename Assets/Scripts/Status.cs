public enum StatusType {
	Sleep =     (1 << 0),
	Poison =    (1 << 1),
    Invisible = (1 << 2), // 透明
    Freeze =    (1 << 3), // かなしばり
    Blind  =    (1 << 4), // 目つぶし
    VisibleAll =   (1 << 5), // よく見え (TODO:アイテム、モンスターのみ場合)
}
