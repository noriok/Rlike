using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
// using System;
// using System.Collections;

using System.Collections.Generic;
using System.Linq;

/*

 j : 南  に移動 (移動先に敵がいる場合は、敵への攻撃)
 k : 北  に移動
 l : 東  に移動
 h : 西  に移動
 b : 南西に移動
 n : 南東に移動
 y : 北西に移動
 u : 北東に移動
 i : アイテムウィンドウを開く
 . : 何もせずにターン終了
 ; : 階段を下りる
 シフトキー + 方向キー : その方向へ矢を放つ(矢が必要)

*/

enum GameState {
    InputWait, // 入力待ち
    Act,       // 行動中

    TurnStart,  // ターン開始
    TurnFinish, // ターン終了
}

public class MainSystem : MonoBehaviour {
    private GameState _gameState;
    private Player _player;
    private List<Enemy> _enemies = new List<Enemy>();

    private List<Act> _actQueue = new List<Act>();

    private Map _map;
    private int _turnCount = 0;

    void Start() {
        _player = CreatePlayer(1, 1);

        var e = EnemyFactory.CreateEnemy(1, 4);
        _enemies.Add(e);
//        _enemies.Add(EnemyFactory.CreateEnemy(2, 2));

        // カメラズーム
        var camera = GameObject.Find("Main Camera");
        camera.GetComponent<Camera>().orthographicSize = 1.5f;

        _map = new Map();

        ChangeGameState(GameState.TurnStart);
    }

    void Update() {
        if (_gameState == GameState.Act) {
            UpdateAct();
            return;
        }

        Assert.IsTrue(_gameState == GameState.InputWait);

        if (Input.GetKeyDown(KeyCode.Space)) {
            ExecutePlayerAttack();
        }
        else if (Input.GetKeyDown(KeyCode.J)) { // 南
            ExecutePlayerMove(1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.K)) { // 北
            ExecutePlayerMove(-1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.L)) { // 東
            ExecutePlayerMove(0, 1);
        }
        else if (Input.GetKeyDown(KeyCode.H)) { // 西
            ExecutePlayerMove(0, -1);
        }
        else if (Input.GetKeyDown(KeyCode.B)) { // 南西
            ExecutePlayerMove(1, -1);
        }
        else if (Input.GetKeyDown(KeyCode.N)) { // 南東
            ExecutePlayerMove(1, 1);
        }
        else if (Input.GetKeyDown(KeyCode.Y)) { // 北西
            ExecutePlayerMove(-1, -1);
        }
        else if (Input.GetKeyDown(KeyCode.U)) { // 北東
            ExecutePlayerMove(-1, 1);
        }
        else if (Input.GetKeyDown(KeyCode.Period)) { // 何もせずターン終了
            ExecutePlayerWait();
        }
    }

    void OnGUI() {
        // Func<string, bool> button = (caption) => {
        //     return GUILayout.Button(caption, GUILayout.Width(110), GUILayout.Height(50));
        // };

        // if (button("Test 1")) {
        // }

        if (GUI.Button(new Rect(600, 0, 100, 40), "test")) {
            var obj = GameObject.Find("hone_3_W");
            Debug.Log(obj);

            var anim = obj.GetComponent<Animator>();
            Debug.Log(anim);

            anim.SetTrigger("ToS"); // アニメーションの切り替え
        }
        else if (GUI.Button(new Rect(600, 40*1, 100, 40), "zoom in")) {
            var camera = GameObject.Find("Main Camera");
            camera.GetComponent<Camera>().orthographicSize -= 0.1f;
        }
        else if (GUI.Button(new Rect(600, 40*2, 100, 40), "zoom out")) {
            var camera = GameObject.Find("Main Camera");
            camera.GetComponent<Camera>().orthographicSize += 0.1f;
        }
    }

    private Player CreatePlayer(int row, int col) {
        var obj = Resources.Load("Prefabs/Character/kabocha_1");
        var gobj = (GameObject)GameObject.Instantiate(obj, Vector3.zero, Quaternion.identity);
        return new Player(row, col, gobj);
    }

    private void ChangeGameState(GameState nextGameState) {
        _gameState = nextGameState;

        switch (nextGameState) {
        case GameState.Act:
            Debug.Log("### Act");
            break;

        case GameState.InputWait:
            Debug.Log("### Input Wait");
            break;

        case GameState.TurnStart:
            Debug.Log("### Turn Start");
            SysTurnStart();

            ChangeGameState(GameState.InputWait);
            break;

        case GameState.TurnFinish:
            Debug.Log("### Turn Finish");
            SysTurnFinish();

            ChangeGameState(GameState.TurnStart);
            break;

        default:
            Assert.IsTrue(false);
            break;
        }
    }

    private void UpdateAct() {
        bool finished = true;
        foreach (var act in _actQueue) {
            act.UpdateAct(this);
            finished = finished && act.Finished;
        }

        if (finished) { // 全てのタスクが終了した
            Debug.Log("--- finished ---");
            _actQueue.Clear();

            // 行動していないキャラがいあるなら、行動を決定する
            bool existsActor = DetectEnemyAct(_player.Loc);
            if (existsActor) {
                Debug.Log("(next actor exists)");
            }
            if (!existsActor) { // 行動するキャラは存在しない
                // 全てのキャラの行動が終了した
                ChangeGameState(GameState.TurnFinish);
            }
        }
    }

    // システム関連

    private void SysTurnStart() {
        _turnCount++;
        DLog.D("ターン: {0}", _turnCount);
    }

    private void SysTurnFinish() {
        var text = GameObject.Find("Text").GetComponent<Text>();
        text.text = DLog.ToText();
        DLog.Clear();

        // 行動回数の復帰
        foreach (var e in _enemies) {
            e.ActCount = 1;
        }
    }

    // プレイヤーの行動

    private void ExecutePlayerMove(int drow, int dcol) {
        Assert.IsTrue(_actQueue.Count == 0);

        Dir dir = Utils.ToDir(drow, dcol);
        Loc to = _player.Loc.Forward(dir);
        bool notExistsEnemy = !_enemies.Where(e => e.Loc.Row == to.Row && e.Loc.Col == to.Col).Any();
        if (_map.CanAdvance(_player.Loc, dir) && notExistsEnemy) {
            Debug.LogFormat("player move delta:{0}", new Loc(drow, dcol));
            _actQueue.Add(new ActPlayerMove(_player, drow, dcol));

            var playerNextLoc = _player.Loc + new Loc(drow, dcol);
            DetectEnemyAct(playerNextLoc);
            ChangeGameState(GameState.Act);
        }
        else {
            Debug.Log("進めません");
        }
    }

    private void ExecutePlayerWait() {
        Assert.IsTrue(_actQueue.Count == 0);

        _actQueue.Add(new ActPlayerWait(_player));
        ChangeGameState(GameState.Act);
    }

    private void ExecutePlayerAttack() {
        Assert.IsTrue(_actQueue.Count == 0);

        _actQueue.Add(new ActPlayerAttack(_player, null));
        ChangeGameState(GameState.Act);
    }

    private bool DetectEnemyAct(Loc playerNextLoc) {
        _actQueue.AddRange(EnemyStrategy.Detect(_enemies, _player, playerNextLoc, _map));
        return _actQueue.Count > 0;
    }
}
