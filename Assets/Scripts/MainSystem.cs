using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections;
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

enum GameMode {
    Wait, // 入力待ち
    Act,  // 行動中
}

public class MainSystem : MonoBehaviour {
    private GameMode _mode;
    private List<Enemy> _enemies = new List<Enemy>();

    private List<ActEnemyWalk> _walkActs = new List<ActEnemyWalk>();

    void Start() {
        _mode = GameMode.Wait;
        // var e = EnemyFactory.CreateEnemy();
        // Debug.Log("e = " + e);
        // var act = new ActEnemyWalk(e);

        var e = EnemyFactory.CreateEnemy();
        _enemies.Add(e);

    }

    void Update() {
        if (_mode == GameMode.Act) {
            GameUpdate();
            return;
        }

        Assert.IsTrue(_mode == GameMode.Wait);

        if (Input.GetKeyDown(KeyCode.J)) { // 南
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
            SkipPlayer();
            DetectEnemyAct();
            ChangeMode(GameMode.Act);
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
        else if (button("Skip")) {

        }
    }

    private void ChangeMode(GameMode mode) {
        switch (mode) {
        case GameMode.Act:
            Debug.Log("### ACT");
            break;

        case GameMode.Wait:
            Debug.Log("### WAIT");
            break;
        }

        _mode = mode;
    }

    private void GameUpdate() {
        foreach (var act in _walkActs) {
            act.Exec(this);
        }

        bool allFinished = true;
        foreach (var act in _walkActs) {
            allFinished = allFinished && act.Finished;
        }

        if (allFinished) {
            Debug.Log("All Finished");
            _walkActs.Clear();
            ChangeMode(GameMode.Wait);
        }
    }

    private void MovePlayer(int drow, int dcol) {

    }

    private void SkipPlayer() {

    }

    private void DetectEnemyAct() {
        Assert.IsTrue(_walkActs.Count == 0);

        foreach (var enemy in _enemies) {
            _walkActs.Add(new ActEnemyWalk(enemy));
        }
    }
}
