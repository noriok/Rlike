using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
// using System;
// using System.Collections;

using System.Collections.Generic;
using System.Linq;

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

    private List<Act> _acts = new List<Act>();

    private Map _map;
    private int _turnCount = 0;

    void Start() {
        _player = CreatePlayer(1, 1);

        _enemies.Add(EnemyFactory.CreateEnemy(1, 4));
        _enemies.Add(EnemyFactory.CreateEnemy(2, 2));

        // カメラズーム
        var camera = GameObject.Find("Main Camera");
        camera.GetComponent<Camera>().orthographicSize = 2.5f;
        // 3.2 => 9 マス
        // 2.8 => 8 マス
        // 2.5 => 7 マス
        // 2.1 => 6 マス
        // 1.74 => 5

        _map = new Map();

        ChangeGameState(GameState.TurnStart);
    }

    void Update() {
        if (_gameState == GameState.Act) {
            UpdateAct();
            return;
        }

        Assert.IsTrue(_gameState == GameState.InputWait);

        if (Input.GetKey(KeyCode.Space)) {
            ExecutePlayerAttack();
        }
        else if (Input.GetKey(KeyCode.J)) { // 南
            ExecutePlayerMove(1, 0);
        }
        else if (Input.GetKey(KeyCode.K)) { // 北
            ExecutePlayerMove(-1, 0);
        }
        else if (Input.GetKey(KeyCode.L)) { // 東
            ExecutePlayerMove(0, 1);
        }
        else if (Input.GetKey(KeyCode.H)) { // 西
            ExecutePlayerMove(0, -1);
        }
        else if (Input.GetKey(KeyCode.B)) { // 南西
            ExecutePlayerMove(1, -1);
        }
        else if (Input.GetKey(KeyCode.N)) { // 南東
            ExecutePlayerMove(1, 1);
        }
        else if (Input.GetKey(KeyCode.Y)) { // 北西
            ExecutePlayerMove(-1, -1);
        }
        else if (Input.GetKey(KeyCode.U)) { // 北東
            ExecutePlayerMove(-1, 1);
        }
        else if (Input.GetKey(KeyCode.Period)) { // 何もせずターン終了
            ExecutePlayerWait();
        }
        else if (Input.GetKey(KeyCode.A)) { // 回復アイテム使う
            ExecutePlayerUseItem();
        }
        else if (Input.GetKey(KeyCode.S)) { // スキル使う
            ExecutePlayerUseSkill();
        }

        var button = GameObject.Find("Canvas/Button_S");
        var b = button.GetComponent<Button2>();
        if (b.Pressed) {
            ExecutePlayerMove(1, 0);
        }
    }

    void OnGUI() {
        // Func<string, bool> button = (caption) => {
        //     return GUILayout.Button(caption, GUILayout.Width(110), GUILayout.Height(50));
        // };

        // if (button("Test 1")) {
        // }

        int x = 300;
        if (GUI.Button(new Rect(x, 0, 100, 40), "test")) {
        }
        else if (GUI.Button(new Rect(x, 40*1, 100, 40), "zoom in")) {
            ZoomIn();
        }
        else if (GUI.Button(new Rect(x, 40*2, 100, 40), "zoom out")) {
            ZoomOut();
        }
    }

    private void ZoomIn() {
        var camera = GameObject.Find("Main Camera");
        camera.GetComponent<Camera>().orthographicSize -= 0.1f;
    }

    private void ZoomOut() {
        var camera = GameObject.Find("Main Camera");
        camera.GetComponent<Camera>().orthographicSize += 0.1f;
    }

    private Player CreatePlayer(int row, int col) {
        // var obj = Resources.Load("Prefabs/Animations/kabocha_0");
        var obj = Resources.Load("Prefabs/Animations/majo_0");
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
       // HP がゼロの敵は削除する
        bool updated = true;
        while (updated) {
            updated = false;
            for (int i = 0; i < _enemies.Count; i++) {
                if (_enemies[i].Hp <= 0) {
                    _enemies.RemoveAt(i);
                    updated = true;
                    break;
                }
            }
        }

        // 移動するキャラのタスクを先に実行する
        bool moveFinished = true;
        foreach (var act in _acts) {
            if (act.Actor.Hp <= 0) continue;
            if (act.IsMoveAct() && !act.Finished) {
                act.UpdateAct(this);
                moveFinished = moveFinished && act.Finished;
            }
        }

        if (!moveFinished) return; // 移動が完了するまで待機

        // 全てのキャラの移動が完了したら、(移動以外の)行動を一つずつ実行する
        bool actFinished = true;
        foreach (var act in _acts) {
            if (act.Actor.Hp <= 0) continue;
            if (act.Finished || act.IsMoveAct()) continue;

            act.UpdateAct(this);
            actFinished = false;
            break; // 行動処理は 1 体ずつ行う
        }

        // 予約されている全てのタスクが終了した
        if (actFinished) {
            // 行動していないキャラの Act を取得
            var acts = DetectEnemyAct(_player.Loc);

            Debug.Log("acts.Count = " + acts.Count);
            if (acts.Count == 0) { // 全てのキャラの行動が終了した
                Debug.Log("行動終了!!!!!!");
                ChangeGameState(GameState.TurnFinish);
            }
            else {
                _acts.AddRange(acts); // Act を追加
            }
        }
    }

    // システム関連

    // ターンスタート直前
    private void SysTurnStart() {
        _turnCount++;
        DLog.D("ターン: {0}", _turnCount);
    }

    // ターン終了後
    private void SysTurnFinish() {
        _acts.Clear();

        var text = GameObject.Find("Canvas/Text").GetComponent<Text>();
        text.text = DLog.ToText();
        DLog.Clear();

        // 行動回数の復帰
        foreach (var e in _enemies) {
            e.ActCount = 1;
        }
    }

    // プレイヤーの行動

    private void ExecutePlayerMove(int drow, int dcol) {
        Assert.IsTrue(_acts.Count == 0);

        Dir dir = Utils.ToDir(drow, dcol);
        Loc to = _player.Loc.Forward(dir);
        bool notExistsEnemy = !_enemies.Where(e => e.Loc.Row == to.Row && e.Loc.Col == to.Col).Any();
        if (_map.CanAdvance(_player.Loc, dir) && notExistsEnemy) {
            _acts.Add(new ActPlayerMove(_player, drow, dcol));

            var playerNextLoc = _player.Loc + new Loc(drow, dcol);
            _acts.AddRange(DetectEnemyAct(playerNextLoc));
            ChangeGameState(GameState.Act);
        }
        else {
            Debug.Log("進めません");
            _player.ChangeDir(dir);
        }
    }

    private void ExecutePlayerWait() {
        Assert.IsTrue(_acts.Count == 0);

        _acts.Add(new ActPlayerWait(_player));
        ChangeGameState(GameState.Act);
    }

    private void ExecutePlayerAttack() {
        Assert.IsTrue(_acts.Count == 0);

        Dir dir = _player.Dir;
        Loc loc = _player.Loc.Forward(dir);

        Enemy target = null;
        Enemy enemy = FindEnemy(loc);
        if (enemy != null && _map.CanAdvance(_player.Loc, dir)) {
            target = enemy;
        }

        _acts.Add(new ActPlayerAttack(_player, target));
        ChangeGameState(GameState.Act);
    }

    private void ExecutePlayerUseItem() {
        Assert.IsTrue(_acts.Count == 0);

        _acts.Add(new ActPlayerUseItem(_player));
        ChangeGameState(GameState.Act);
    }

    private void ExecutePlayerUseSkill() {
        Assert.IsTrue(_acts.Count == 0);

        _acts.Add(new ActPlayerUseSkill(_player, _enemies.ToArray()));
        ChangeGameState(GameState.Act);
    }

    private List<Act> DetectEnemyAct(Loc playerNextLoc) {
        return EnemyStrategy.Detect(_enemies, _player, playerNextLoc, _map);
    }

    private Enemy FindEnemy(Loc loc) {
        foreach (var e in _enemies) {
            if (e.Row == loc.Row && e.Col == loc.Col) {
                return e;
            }
        }
        return null;
    }
}
