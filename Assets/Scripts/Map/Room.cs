using UnityEngine;
using System;
using System.Collections.Generic;

public class Room {
	public int Row { get; private set; }
	public int Col { get; private set; }
	public int Width { get; private set; }
	public int Height { get; private set; }
	public Loc[] Entrances { get; private set; }

	public Room(int row, int col, int width, int height, List<Loc> entrances) {
		Row = row;
		Col = col;
		Width = width;
		Height = height;
		Entrances = entrances.ToArray();

		Debug.LogFormat("row:{0} col:{1} width:{2} height:{3}", row, col, width, height);
		foreach (var e in Entrances) {
			Debug.Log("entrance: " + e);
		}
	}

	public bool IsEntrance(Loc loc) {
		return Array.IndexOf(Entrances, loc) >= 0;
	}
}
