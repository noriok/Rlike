using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room {
	private List<Loc> _entrances;
	private int _row;
	private int _col;
	private int _width;
	private int _height;

	public Room(int row, int col, int width, int height, List<Loc> entrances) {
		_row = row;
		_col = col;
		_width = width;
		_height = height;

		_entrances = entrances;

		Debug.LogFormat("row:{0} col:{1} width:{2} height:{3}", _row, _col, _width, _height);
		foreach (var e in _entrances) {
			Debug.Log("entrance: " + e);
		}
	}
}
