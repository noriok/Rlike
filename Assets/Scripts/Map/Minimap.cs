using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

public class Minimap {
	private const float Size = 0.32f; // 32x32

	private GameObject _layer;

	private GameObject _playerIcon;
	private List<GameObject> _enemyIcons = new List<GameObject>();

	private float _elapsed;

	public Minimap(char[,] map, List<FieldObject> fieldObjects, Loc stairsLoc) {
		_layer = new GameObject(LayerName.Minimap);

		int rows = map.GetLength(0);
		int cols = map.GetLength(1);
		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < cols; j++) {
				if (map[i, j] == MapChar.Room || map[i, j] == MapChar.Passage) {
					CreateFloor(i, j, _layer);
				}
			}
		}

		// trap
		foreach (var fo in fieldObjects) {
			if (fo is Trap) {
				CreateTrap(fo.Loc.Row, fo.Loc.Col, _layer);
			}
		}

		// 階段 TODO:階段の位置を引数で受け取る
		CreateStairs(stairsLoc.Row, stairsLoc.Col, _layer);

		_playerIcon = CreatePlayer(0, 0, _layer);
		for (int i = 0; i < Config.EnemyMaxCount; i++) {
			_enemyIcons.Add(CreateEnemy(0, 0, _layer));
		}
		// UpdateIcon(playerLoc, enemies);

		// for (int i = 0; i < enemies.Count; i++) {
		// 	_enemyIcons[i].transform.position = ToPosition(enemies[i].Loc);
		// 	_enemyIcons[i].SetActive(true);
		// }

		_layer.transform.localScale = new Vector3(0.1f, 0.1f, 1.0f);
		_layer.transform.position = new Vector3(-0.8f, 0.3f, 0);
	}

	public void UpdateIcon(Loc playerLoc, List<Enemy> enemies) {
		_playerIcon.transform.localPosition = ToPosition(playerLoc);

		for (int i = 0; i < _enemyIcons.Count; i++) {
			if (i < enemies.Count) {
				_enemyIcons[i].SetActive(true);
				_enemyIcons[i].transform.localPosition = ToPosition(enemies[i].Loc);
			}
			else {
				_enemyIcons[i].SetActive(false);
			}
		}
	}

	// プレイヤーアイコンの点滅更新
	public void UpdatePlayerIconBlink() {
		float blinkTime = 0.6f;
		_elapsed += Time.deltaTime;
		if (_elapsed >= blinkTime) {
			_playerIcon.SetActive(!_playerIcon.activeSelf);
			_elapsed -= blinkTime;
		}
	}

	public void Show() {
		_layer.SetActive(true);
	}

	public void Hide() {
		_layer.SetActive(false);
	}

	private Vector3 ToPosition(Loc loc) {
		return new Vector3(loc.Col * Size, -loc.Row * Size, 0);
	}

	private GameObject Create(string path, int row, int col, GameObject layer) {
		var obj = Resources.Load(path);
		var pos = ToPosition(new Loc(row, col));
		var gobj = (GameObject)GameObject.Instantiate(obj, pos, Quaternion.identity);
		gobj.transform.SetParent(layer.transform);
		return gobj;
	}

	private GameObject CreateFloor(int row, int col, GameObject layer) {
		var gobj = Create("Prefabs/Minimap/minimap-floor", row, col, layer);
		var renderer = gobj.GetComponent<SpriteRenderer>();
		var color = renderer.color;
		color.a = 140f / 255f;
		renderer.color = color;
		return gobj;
	}

	private GameObject CreateTrap(int row, int col, GameObject layer) {
		return Create("Prefabs/Minimap/minimap-trap", row, col, layer);
	}

	private GameObject CreatePlayer(int row, int col, GameObject layer) {
		return Create("Prefabs/Minimap/minimap-player", row, col, layer);
	}

	private GameObject CreateEnemy(int row, int col, GameObject layer) {
		return Create("Prefabs/Minimap/minimap-enemy", row, col, layer);
	}

	private GameObject CreateStairs(int row, int col, GameObject layer) {
		return Create("Prefabs/Minimap/minimap-stairs", row, col, layer);
	}
}
