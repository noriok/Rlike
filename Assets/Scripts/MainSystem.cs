using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
// using System;
using System.Collections;

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
 . : 何もせずにターン終了

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

    IEnumerator PopupDigits(int n, Vector3 pos) {
        float fontWidth = 0.14f;

        var digits = new List<GameObject>();
        var ds = Utils.Digits(n);
        float x = pos.x - fontWidth * ds.Length / 2.0f + fontWidth / 2;
        foreach (var d in ds) {
            var obj = Resources.Load("Prefabs/Digits/digits_green_" + d);
            var gobj = (GameObject)Instantiate(obj, new Vector3(x, pos.y, pos.z), Quaternion.identity);
            digits.Add(gobj);
            x += fontWidth;
        }

        float v = -0.059f; // velocity
        float g = 0.008f; // gravity
        float elapsed = 0;

        int frame = 0;
        float y = pos.y;
        while (true) {
            int f = (int)(elapsed / 0.033f);
            if (frame < f) {
                frame++;
                y -= v;
                v += g;
                if (y <= pos.y) {
                    v *= -0.45f;
                    y = 0;

                    if (Mathf.Abs(v) < 0.016f) {
                        v = 0;
                        foreach (var digit in digits) {
                            var p = digit.transform.position;
                            p.y = y;
                            digit.transform.position = p;
                        }
                        yield return new WaitForSeconds(0.8f);
                        foreach (var digit in digits) {
                            Destroy(digit);
                        }
                        yield break;
                    }
                }

                foreach (var digit in digits) {
                    var p = digit.transform.position;
                    p.y = y;
                    digit.transform.position = p;
                }
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    void Start() {
        _player = CreatePlayer(1, 1);

        _enemies.Add(EnemyFactory.CreateEnemy(1, 4));
        _enemies.Add(EnemyFactory.CreateEnemy(2, 2));

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
    }

    void OnGUI() {
        // Func<string, bool> button = (caption) => {
        //     return GUILayout.Button(caption, GUILayout.Width(110), GUILayout.Height(50));
        // };

        // if (button("Test 1")) {
        // }

        if (GUI.Button(new Rect(600, 0, 100, 40), "test")) {
            var n = new System.Random().Next(100);
            StartCoroutine(PopupDigits(n, Vector3.zero));
        }
        else if (GUI.Button(new Rect(600, 40*1, 100, 40), "zoom in")) {
            ZoomIn();
        }
        else if (GUI.Button(new Rect(600, 40*2, 100, 40), "zoom out")) {
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
        var obj = Resources.Load("Prefabs/Animations/kabocha_0");
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
