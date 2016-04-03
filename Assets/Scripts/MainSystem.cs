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
 a : HP 表示メータのオン/オフ (デフォルトでオン)

※「方向キー」とは、上の 8 つのキー(j,k,l,h,b,n,y,u)のことです。

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
    Wait, // 入力待ち
    Act,  // 行動中

    TurnFinished, // ターン終了
}

public class MainSystem : MonoBehaviour {
    private GameState _gameState;
    private List<Enemy> _enemies = new List<Enemy>();

    private List<Act> _actQueue = new List<Act>();

    void Start() {
        _gameState = GameState.Wait;

        var e = EnemyFactory.CreateEnemy(0, 0);
        _enemies.Add(e);
        _enemies.Add(EnemyFactory.CreateEnemy(1, 0));

    }

    void Update() {
        if (_gameState == GameState.Act) {
            UpdateAct();
            return;
        }

        Assert.IsTrue(_gameState == GameState.Wait);

        if (Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log("player attack");
        }
        else if (Input.GetKeyDown(KeyCode.J)) { // 南
        }
        else if (Input.GetKeyDown(KeyCode.K)) { // 北
        }
        else if (Input.GetKeyDown(KeyCode.L)) { // 東
        }
        else if (Input.GetKeyDown(KeyCode.H)) { // 西
        }
        else if (Input.GetKeyDown(KeyCode.B)) { // 南西
        }
        else if (Input.GetKeyDown(KeyCode.N)) { // 南東
        }
        else if (Input.GetKeyDown(KeyCode.Y)) { // 北西
        }
        else if (Input.GetKeyDown(KeyCode.U)) { // 北東
        }
        else if (Input.GetKeyDown(KeyCode.Period)) { // 何もせずターン終了
            Debug.Log("SKIP PLAYER TURN");
            ExecutePlayerSkip();
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

    private void ChangeGameState(GameState nextGameState) {
        _gameState = nextGameState;

        switch (nextGameState) {
        case GameState.Act:
            Debug.Log("### ACT");
            break;

        case GameState.Wait:
            Debug.Log("### WAIT");
            break;

        case GameState.TurnFinished:
            Debug.Log("### TURN FINISHED");
            SysFinishTurn();
            ChangeGameState(GameState.Wait);
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
            Debug.Log("all finished");
            _actQueue.Clear();

            // 行動していないキャラがいあるなら、行動を決定する
            bool existsActor = DetectEnemyAct();
            Debug.Log("existsActor = " + existsActor);
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
    }

    private void ExecutePlayerSkip() {
        DetectEnemyAct();
        ChangeGameState(GameState.Act);
    }

    private void ExecutePlayerAttack() {

    }

    private bool DetectEnemyAct() {
        Assert.IsTrue(_actQueue.Count == 0);

        var q = new List<Act>();
        foreach (var enemy in _enemies) {
            if (enemy.ActCount > 0) {
                q.Add(new ActEnemyWalk(enemy));
            }
        }

        // 優先順位のもっとも高いもののみキューに残してそれ以外は破棄
        // (移動と、攻撃の処理順番の制御)
        int priority = -1;
        foreach (var act in q) {
            if (priority < act.Priority) {
                priority = act.Priority;
                _actQueue.Clear();
            }

            if (priority == act.Priority) {
                _actQueue.Add(act);
            }
        }
        return _actQueue.Count > 0;
    }
}
