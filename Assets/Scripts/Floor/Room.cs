// using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class Room {
    public int Id { get; private set; }
	public int Row { get; private set; } // 上端
	public int Col { get; private set; } // 左端
    public int EndRow { get { return Row + Height - 1; } } // 下端
    public int EndCol { get { return Col + Width - 1; } }  // 右端
	public int Width { get; private set; }
	public int Height { get; private set; }
	public Loc[] Entrances { get; private set; }
    public Loc[] Corners {
        get {
            // 左上から時計回り
            return new[] {
                new Loc(Row, Col),
                new Loc(Row, EndCol),
                new Loc(EndRow, EndCol),
                new Loc(EndRow, Col),
            };
        }
    }

    // 四隅の一マス外側の座標
    public Loc[] OutsideCorners {
        get {
            return Corners.Zip(new[] { Dir.NW, Dir.NE, Dir.SE, Dir.SW }, (loc, dir) => loc.Forward(dir)).ToArray();
        }
    }

	public Room(int id, int row, int col, int width, int height, List<Loc> entrances) {
        Id = id;
		Row = row;
		Col = col;
		Width = width;
		Height = height;
		Entrances = entrances.ToArray();

		// Debug.LogFormat("row:{0} col:{1} width:{2} height:{3}", row, col, width, height);
		// foreach (var e in Entrances) {
		// 	Debug.Log("entrance: " + e);
		// }
	}

	public bool IsEntrance(Loc loc) {
		return Array.IndexOf(Entrances, loc) >= 0;
	}

    public override string ToString() {
        return string.Format("Room row:{0} col:{1} width:{2} height:{3}", Row, Col, Width, Height);
    }

    public bool IsInside(Loc loc, bool isContainsOuter = false) {
        if (Row <= loc.Row && loc.Row <= EndRow &&
            Col <= loc.Col && loc.Col <= EndCol)
        {
            return true;
        }

        // 部屋の周囲一マスも部屋の中と見なす
        if (isContainsOuter) {
            // 部屋のスポットライトは、外周一マスなので
            if (Row <= loc.Row && loc.Row <= EndRow) {
                return loc.Col == Col - 1 || loc.Col == EndCol + 1;
            }
            else if (Col <= loc.Col && loc.Col <= EndCol) {
                return loc.Row == Row - 1 || loc.Row == EndRow + 1;
            }
        }
        return false;
    }

    public Loc RandomLoc() {
        int r = Row + Rand.Next(Height);
        int c = Col + Rand.Next(Width);
        return new Loc(r, c);
    }
}
