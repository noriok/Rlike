using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class FloorCreator {
    private const string TestMap = @"
               ##########
               .........
#########      ..~~~~~..
#.......#      ..~...~..
#.......+++++++..~...~..
#.......#      ..~~~~~..
#.......#      .........
#.......#          +
###  +             +
                   +
                ......
                ......
                ......
                ......
                ......




####
#####
";

    private const string Map2 = @"
##################
  ..........
  ..........       ...
  ..........+++++++...
  ..........       ...
  ..........
##################
";

    private const string Map3 = @"
######
 ........
 ........
 ........
 ........
 ########
";

	private static char[,] CreateMap(int floorNumber) {
        string mapData = floorNumber == 1 ? TestMap : Map2;
        mapData = TestMap;
        var lines = mapData.Trim().Split(new[] { '\n' });

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
		var fieldObjectLayer = LayerManager.GetLayer(LayerName.FieldObject);
		var trapLayer = LayerManager.GetLayer(LayerName.Trap);

		// マップ生成
		var mapData = CreateMap(floorNumber);
		Map map = new Map(mapData);

		// マップ上のフィールドオブジェクト作成
        var fieldObjects = new List<FieldObject>();
        if (floorNumber == 1) {
      	    // fieldObjects.Add(FieldObjectFactory.CreateBonfire(new Loc(3, 3), fieldObjectLayer));
            // fieldObjects.Add(FieldObjectFactory.CreateTreasure(new Loc(4, 4), fieldObjectLayer));
            fieldObjects.Add(FieldObjectFactory.CreateNoticeBoard(new Loc(1, 2), fieldObjectLayer, "立て札のメッセージ"));

            fieldObjects.Add(FieldObjectFactory.CreateNoticeBoard(new Loc(2, 15), fieldObjectLayer, "「場所替えの杖」を使って、\n孤島の敵と入れ替わろう！\n\n杖は何回でも使えるぞ！"));
            fieldObjects.Add(FieldObjectFactory.CreateNoticeBoard(new Loc(3, 19), fieldObjectLayer, "「水がれの書」を使うと、\nフロアの水が涸れるぞ！"));
            fieldObjects.Add(FieldObjectFactory.CreateNoticeBoard(new Loc(10, 20), fieldObjectLayer, "部屋の真ん中にあるのは\n召喚のワナです。\n\n「消え去り草」を使うと、\nしばらくの間、姿が見えなくなるぞ！"));


        }

        Loc stairsLoc = new Loc(1, 7);
        if (floorNumber == 1) {
		    // 階段生成
		    // Loc stairsLoc = new Loc(1, 7);
        }
        else {

        }
        FieldObjectFactory.CreateStairs(stairsLoc, fieldObjectLayer);


        // ワナ生成
        if (floorNumber == 1) {
            fieldObjects.Add(FieldObjectFactory.CreateTrapSummon(new Loc(12, 19), trapLayer));


        }
//        fieldObjects.Add(FieldObjectFactory.CreateTrapHeal(new Loc(3, 5), trapLayer));
//        fieldObjects.Add(FieldObjectFactory.CreateTrapWarp(new Loc(3, 6), trapLayer));
//        fieldObjects.Add(FieldObjectFactory.CreateTrapDamage(new Loc(3, 7), trapLayer));
//        fieldObjects.Add(FieldObjectFactory.CreateTrapSummon(new Loc(3, 8), trapLayer));

		// ミニマップ生成
		Minimap minimap = new Minimap(mapData, fieldObjects, stairsLoc);

		var floor = new Floor(map, minimap, fieldObjects, stairsLoc);
		return floor;
	}
}
