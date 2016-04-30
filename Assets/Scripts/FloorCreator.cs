using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class FloorCreator {
    private const string TestMap = @"
#
#...............#      ..........
#...............+++++++..........
#...............#      ..........
#...............#      ..........
#####+       +         ..........
    #+       +         ..........
     +   .......           +
     ++++.......++++++++++++
         .......





####
#####
";
	private static char[,] CreateMap() {
        var lines = TestMap.Trim().Split(new[] { '\n' });

        int rows = lines.Length;
        int cols = lines.Select(s => s.Length).Max();

        var map = new char[rows, cols];
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                map[i, j] = MapChar.Wall;
            }
            for (int j = 0; j < lines[i].Length; j++) {
                if (lines[i][j] == MapChar.None) continue;
                map[i, j] = lines[i][j];
            }
        }
		return map;
	}

	public static Floor CreateFloor(int floorNumber) {
		// var floorLayer = new GameObject(LayerName.Floor);

		var fieldObjectLayer = new GameObject(LayerName.FieldObject);
		var trapLayer = new GameObject(LayerName.Trap);

		// マップ生成
		var mapData = CreateMap();
		Map map = new Map(mapData);

		// マップ上のフィールドオブジェクト作成
		var fieldObjects = new List<FieldObject>();
      	fieldObjects.Add(FieldObjectFactory.CreateBonfire(new Loc(3, 3), fieldObjectLayer));
        fieldObjects.Add(FieldObjectFactory.CreateTreasure(new Loc(4, 4), fieldObjectLayer));
        fieldObjects.Add(FieldObjectFactory.CreateNoticeBoard(new Loc(1, 2), fieldObjectLayer, "立て札のメッセージ"));

		// 階段生成
		Loc stairsLoc = new Loc(1, 7);
		FieldObjectFactory.CreateStairs(stairsLoc, fieldObjectLayer);

        // ワナ生成
        fieldObjects.Add(FieldObjectFactory.CreateTrapHeal(new Loc(3, 5), trapLayer));
        fieldObjects.Add(FieldObjectFactory.CreateTrapWarp(new Loc(3, 6), trapLayer));
        fieldObjects.Add(FieldObjectFactory.CreateTrapDamage(new Loc(3, 7), trapLayer));
        fieldObjects.Add(FieldObjectFactory.CreateTrapSummon(new Loc(3, 8), trapLayer));

		// ミニマップ生成
		Minimap minimap = new Minimap(mapData, fieldObjects, stairsLoc);

		var floor = new Floor(map, minimap, fieldObjects, stairsLoc);
		return floor;
	}
}
