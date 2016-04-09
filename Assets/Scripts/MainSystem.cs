using UnityEngine;
using UnityEngine.Assertions;
using System;
// using System.Collections;
using System.Collections.Generic;

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

** アイテムウィンドウを開いて「いる」とき:

 h : アイテムを使用(または装備)
 i : アイテムウィンドウを閉じる
 j : カーソルを下に移動
 k : カーソルを上に移動
 o : アイテムをソート
 ; : アイテムを置く
 シフトキー + 方向キー : アイテムをその方向へ投げる

*/

enum GameState {
    InputWait, // 入力待ち
    Act,       // 行動中

    TurnFinished, // ターン終了
}

public class MainSystem : MonoBehaviour {
    private GameState _gameState;
    private Player _player;
    private List<Enemy> _enemies = new List<Enemy>();

    private List<Act> _actQueue = new List<Act>();

    void Start() {
        _gameState = GameState.InputWait;

        // (50, 50) にする。カメラを移動させる
        // マップチップを表示させる
        _player = CreatePlayer(0, 0);

        var e = EnemyFactory.CreateEnemy(0, 1);
        _enemies.Add(e);
        _enemies.Add(EnemyFactory.CreateEnemy(2, 2));
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
            Debug.Log("SKIP PLAYER TURN");
            ExecutePlayerWait();
        }
    }

    void OnGUI() {
        Func<string, bool> button = (caption) => {
            return GUILayout.Button(caption, GUILayout.Width(110), GUILayout.Height(50));
        };

        if (button("Test 1")) {
        }
        else if (button("Test 2")) {
        }
        else if (button("Test 3")) {
        }
    }

    private Player CreatePlayer(int row, int col) {
        var obj = Resources.Load("Prefabs/piece1");
        var gobj = (GameObject)GameObject.Instantiate(obj, Vector3.zero, Quaternion.identity);
        return new Player(row, col, gobj);
    }

    private void ChangeGameState(GameState nextGameState) {
        _gameState = nextGameState;

        switch (nextGameState) {
        case GameState.Act:
            Debug.Log("### ACT");
            break;

        case GameState.InputWait:
            Debug.Log("### INPUT WAIT");
            break;

        case GameState.TurnFinished:
            Debug.Log("### TURN FINISHED");
            SysFinishTurn();
            ChangeGameState(GameState.InputWait);
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
                ChangeGameState(GameState.TurnFinished);
            }
        }
    }

    // システム関連

    private void SysFinishTurn() {

        // 行動回数の復帰
        foreach (var e in _enemies) {
            e.ActCount = 1;
        }
    }

    // プレイヤーの行動

    private void ExecutePlayerMove(int drow, int dcol) {
        Assert.IsTrue(_actQueue.Count == 0);

        Debug.LogFormat("player move delta:{0}", new Loc(drow, dcol));
        _actQueue.Add(new ActPlayerMove(_player, drow, dcol));

        var playerNextLoc = _player.Loc + new Loc(drow, dcol);
        DetectEnemyAct(playerNextLoc);
        ChangeGameState(GameState.Act);
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
        // 捨てられる act をログに出力する

        // foreach (var enemy in _enemies) {
        //     if (enemy.ActCount > 0) {
        //         _actQueue.Add(EnemyStrategy.Simple(enemy, _player, playerNextLoc));
        //     }
        // }
        _actQueue.AddRange(EnemyStrategy.Detect(_enemies, _player, playerNextLoc));

        return _actQueue.Count > 0;
    }
}
