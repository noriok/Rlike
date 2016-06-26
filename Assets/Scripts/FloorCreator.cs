using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class FloorCreator {
    private class Data {
        public string Map { get; private set; }
        public List<FieldObject> FieldObjects { get; private set; }
        public Loc StairsLoc { get; private set; }

        public Loc PlayerLoc { get; private set; }

        public List<FieldItem> FieldItems { get; private set; }
        public List<Enemy> Enemies { get; private set; }

        public Data(string map, Loc stairsLoc, Loc playerLoc, List<FieldObject> fieldObjects, List<FieldItem> fieldItems, List<Enemy> enemies) {
            Map = map;
            StairsLoc = stairsLoc;
            PlayerLoc = playerLoc;
            FieldObjects = fieldObjects;
            FieldItems = fieldItems;
            Enemies = enemies;
        }
    }

    private const string TestMap = @"
               ##########
               .........
#########      ..~~~~~..
#.......#      ..~...~..
#.......+++++++..~...~..
#.......#      ..~~~~~..
#.......#      .........
#.......#          +         ....###
### +              +         ..~~###
    +              +     ++++..~.###
    +           ......   +   ..~~###
  .....         ......   +   ....###
  .....         ......++++
  .....+++++++++......
  .....         ......
####
#####
";

    private const string Map3 = @"
.~.~.~.~
~.~.~.~.
.~.~.~.~
........
########
";

    // TODO:削除
    private static char[,] CreateMap(int floorNumber) {
        string mapData = Map3;
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

    private static char[,] CreateMap(string mapData) {
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

    private static Data GetData(int floorNumber) {
        switch (floorNumber) {
        case 1: return D1();
        case 2: return D2();
        case 3: return D3();
        case 4: return D4();
        case 5: return D5();
        case 6: return D6();
        case 7: return D7();
        case 8: return D8();
        case 9: return D9();
        }
        Assert.IsTrue(false);
        return null;
    }

    public static Floor CreateFloor(int floorNumber, Player player, MainSystem sys) {
        // プレイヤーの状態をリセット
        player.ClearItems();
        player.UpdateHp(player.MaxHp);

        Data data = D1_0();
        // Data data = GetData(floorNumber);
        char[,] mapData = CreateMap(data.Map);
        var map = new Map(mapData);

        foreach (var fitem in data.FieldItems) {
            sys.AddFieldItem(fitem);
        }

        foreach (var enemy in data.Enemies) {
            sys.AddEnemy(enemy);
        }

        FieldObjectFactory.CreateStairs(data.StairsLoc);

        // ミニマップ生成
        Minimap minimap = new Minimap(mapData, data.FieldObjects, data.StairsLoc);

        var floor = new Floor(map, minimap, data.FieldObjects, data.StairsLoc);
        player.UpdateLoc(data.PlayerLoc); // TODO:プレイヤーの初期位置設定
        player.ChangeDir(Dir.S);
        return floor;
    }

    // ------ フロアデータ

    private static Data D1_0() {
        const string map = TestMap;

        var stairsLoc = new Loc(4, 3);
        var playerLoc = new Loc(5, 3);

        var fobjs = new List<FieldObject>();
        fobjs.Add(FieldObjectFactory.CreateTrapHeal(new Loc(6, 6)));

        var fitems = new List<FieldItem>();
        fitems.Add(FieldItemFactory.CreateMagic(new Loc(6, 3), 4)); // 水がれ
        fitems.Add(FieldItemFactory.CreateHerb(new Loc(6, 4), 1)); // 睡眠草
        fitems.Add(FieldItemFactory.CreateHerb(new Loc(6, 5), 4)); // 目薬草


        var enemies = new List<Enemy>();
        enemies.Add(EnemyFactory.CreateEnemy(new Loc(5, 5)));
        enemies.Last().AddStatus(StatusType.Sleep);

        return new Data(map, stairsLoc, playerLoc, fobjs, fitems, enemies);
    }

    private static Data D1() {
        const string map = @"
.......
.......
.......
.......
.......
.......
";

        var stairsLoc = new Loc(0, 3);
        var playerLoc = new Loc(2, 3);

        var fobjs = new List<FieldObject>();
        fobjs.Add(FieldObjectFactory.CreateNoticeBoard(new Loc(0, 4), "落ちているアイテムやワナを駆使して\n階段を目指してください。\nフロアを移動すると、\n所持アイテムはなくなります。\n\n画面右上の「ギブアップ」ボタンを押すと\n現在のフロアをやり直します。"));
        // fobjs.Add(FieldObjectFactory.CreateTrapHeal(new Loc(2, 4)));

        var fitems = new List<FieldItem>();

        // fitems.Add(FieldItemFactory.CreateWand(new Loc(2, 3), 3));
        // fitems.Add(FieldItemFactory.CreateWand(new Loc(3, 2), 0));
        // fitems.Add(FieldItemFactory.CreateWand(new Loc(3, 3), 1));
        // fitems.Add(FieldItemFactory.CreateWand(new Loc(3, 4), 2));


        fitems.Add(FieldItemFactory.CreateHerb(new Loc(2, 3), 0));
        var enemies = new List<Enemy>();

        return new Data(map, stairsLoc, playerLoc, fobjs, fitems, enemies);
    }

    private static Data D2() {
        // 斜め移動の練習
        const string map = @"
...~.~.~.
....#.#.~
...~...~.
~.#.~#~.~
.~.~.~.~.
~.~#~.#.~
.~...~...
~.#.#....
.~.~.~...
";
        var fobjs = new List<FieldObject>();
        var stairsLoc = new Loc(7, 7);

        var playerLoc = new Loc(1, 1);

        var fieldItems = new List<FieldItem>();
        var enemies = new List<Enemy>();

        return new Data(map, stairsLoc, playerLoc, fobjs, fieldItems, enemies);
    }

    private static Data D3() {
        // いかずちの杖を振って敵を倒す。
        // 2 体いる。使用回数は 1 回
        const string map = @"
....~...
....~.~.
....~.~.
....~.~~
........
";

        var fobjs = new List<FieldObject>();
        var stairsLoc = new Loc(2, 7);

        var playerLoc = new Loc(2, 1);

        var fitems = new List<FieldItem>();
        // いかずちの杖
        fitems.Add(FieldItemFactory.CreateWand(new Loc(3, 1), 3));

        var enemies = new List<Enemy>();
        enemies.Add(EnemyFactory.CreateEnemy(new Loc(2, 5)));
        enemies.Add(EnemyFactory.CreateEnemy(new Loc(1, 1)));

        return new Data(map, stairsLoc, playerLoc, fobjs, fitems, enemies);
    }

    private static Data D4() {
        // 場所替えで敵と位置を入れ替わって階段に向かう
        const string map = @"
...~...~...
...~...~...
...~...~...
...~...~...
...~...~...
";

        var fobjs = new List<FieldObject>();
        var stairsLoc = new Loc(2, 10);

        var playerLoc = new Loc(2, 1);

        var fitems = new List<FieldItem>();
        // 場所替えの杖
        fitems.Add(FieldItemFactory.CreateWand(new Loc(2, 2), 0));

        var enemies = new List<Enemy>();
        enemies.Add(EnemyFactory.CreateEnemy(new Loc(2, 4)));
        enemies.Add(EnemyFactory.CreateEnemy(new Loc(2, 8)));

        foreach (var e in enemies) {
            e.ChangeDir(Dir.W);
        }
        return new Data(map, stairsLoc, playerLoc, fobjs, fitems, enemies);
    }

    private static Data D5() {
        // かなしばりで敵の進行をふさぐ
        const string map = @"
.......
.~~.~~.
.~...~.
.~...~.
.~~~~~.
.......
";

        var fobjs = new List<FieldObject>();
        var stairsLoc = new Loc(0, 3);

        var playerLoc = new Loc(5, 3);

        var fitems = new List<FieldItem>();
        // かなしばりの杖
        fitems.Add(FieldItemFactory.CreateWand(new Loc(5, 5), 4));

        var enemies = new List<Enemy>();
        enemies.Add(EnemyFactory.CreateEnemy(new Loc(3, 2)));
        enemies.Add(EnemyFactory.CreateEnemy(new Loc(3, 3)));
        enemies.Add(EnemyFactory.CreateEnemy(new Loc(3, 4)));
        return new Data(map, stairsLoc, playerLoc, fobjs, fitems, enemies);
    }


    private static Data D6() {
        // めぐすり草を使う。ワープのワナが見える。
        // TODO:ワープは必ずいまいる部屋とは別の部屋にワープする
        const string map = @"
....##....#
....##....#
....##....#
....##....#
";
        var stairsLoc = new Loc(2, 6);
        var playerLoc = new Loc(1, 3);

        var fobjs = new List<FieldObject>();
        fobjs.Add(FieldObjectFactory.CreateTrapWarp(new Loc(0, 0)));

        var fitems = new List<FieldItem>();
        // めぐすり草
        fitems.Add(FieldItemFactory.CreateHerb(new Loc(1, 1), 4));

        var enemies = new List<Enemy>();
        return new Data(map, stairsLoc, playerLoc, fobjs, fitems, enemies);
    }

    private static Data D7() {
        // 矢をうってくる敵
        const string map = @"
...........
..~........
...........
..#........
...........
";

        var stairsLoc = new Loc(2, 10);
        var playerLoc = new Loc(2, 1);

        var fobjs = new List<FieldObject>();
        var fitems = new List<FieldItem>();

        var enemies = new List<Enemy>();
        enemies.Add(EnemyFactory.CreateEnemy(new Loc(2, 4)));
        enemies.Last().CanLongDistanceAttack = true;

        return new Data(map, stairsLoc, playerLoc, fobjs, fitems, enemies);
    }

    private static Data D8() {
        // TODO: 上下左右いずれの方向に進んでも全ての敵が回りにくるロジック
        const string map = @"
.........
.........
..~...~..
.........    ...
.........++++...
.........    ...
..~...~..
.........
.........
";

        var stairsLoc = new Loc(4, 14);
        var playerLoc = new Loc(4, 4);

        var fobjs = new List<FieldObject>();
        // 地雷のワナ
        fobjs.Add(FieldObjectFactory.CreateTrapLandmine(new Loc(4, 4)));

        var fitems = new List<FieldItem>();
        // めぐすり草
        fitems.Add(FieldItemFactory.CreateHerb(new Loc(5, 4), 4));

        var enemies = new List<Enemy>();
        enemies.Add(EnemyFactory.CreateEnemy(new Loc(4, 8)));
        enemies.Add(EnemyFactory.CreateEnemy(new Loc(4, 0)));
        enemies.Add(EnemyFactory.CreateEnemy(new Loc(1, 1)));
        enemies.Add(EnemyFactory.CreateEnemy(new Loc(1, 7)));
        enemies.Add(EnemyFactory.CreateEnemy(new Loc(7, 1)));
        enemies.Add(EnemyFactory.CreateEnemy(new Loc(7, 7)));
        enemies.Add(EnemyFactory.CreateEnemy(new Loc(0, 4)));
        enemies.Add(EnemyFactory.CreateEnemy(new Loc(8, 4)));

        return new Data(map, stairsLoc, playerLoc, fobjs, fitems, enemies);
    }
/*
    private static Data D9() {
        // ふきとばし。場所替え
        const string map = @"
.....
.....
..~..
.....
.....
.....
  +
  +    ......
  +++++......
       ......
";

        var stairsLoc = new Loc(4, 14);
        var playerLoc = new Loc(1, 2);

        var fobjs = new List<FieldObject>();

        var fitems = new List<FieldItem>();
        //fitems.Add(FieldItemFactory.CreateHerb(new Loc(5, 4), 4));

        var enemies = new List<Enemy>();
        enemies.Add(EnemyFactory.CreateEnemy(new Loc(4, 2)));


        return new Data(map, stairsLoc, playerLoc, fobjs, fitems, enemies);
    }
*/

    private static Data D9() {
        // TODO: 上下左右いずれの方向に進んでも全ての敵が回りにくるロジック
        // ふきとばし。場所替え
        // とびつきで
        const string map = @"
........
........
........
.~~~~~~.
.~....~.
.~....~.
.~....~.
.~~~~~~.
........
";

        var stairsLoc = new Loc(5, 4);
        var playerLoc = new Loc(0, 3);

        var fobjs = new List<FieldObject>();
        fobjs.Add(FieldObjectFactory.CreateNoticeBoard(new Loc(5, 3), "ゲームクリアです！！ \n\n階段を降りると\n1Fに戻ります。"));

        var fitems = new List<FieldItem>();
        fitems.Add(FieldItemFactory.CreateWand(new Loc(2, 3), 1));
        fitems.Add(FieldItemFactory.CreateWand(new Loc(2, 4), 2));

        var enemies = new List<Enemy>();
        enemies.Add(EnemyFactory.CreateEnemy(new Loc(6, 5)));

        return new Data(map, stairsLoc, playerLoc, fobjs, fitems, enemies);
    }
}
