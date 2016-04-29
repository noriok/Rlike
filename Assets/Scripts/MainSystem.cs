using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
// using System;

enum GameState {
    InputWait, // 入力待ち
    Act,       // 行動中

    TurnStart,  // ターン開始
    TurnEnd, // ターン終了
}

public class MainSystem : MonoBehaviour {
    private CameraManager _cameraManager = new CameraManager();
    private GameState _gameState;
    private Player _player;
    private List<Enemy> _enemies = new List<Enemy>();

    private List<Act> _acts = new List<Act>();

    private Dialog _dialog;

    private int _turnCount = 0;

    private KeyPad _keyPad;
    private FloorBanner _banner;
    private GameManager _gm;
    private Floor _floor;

    void Start() {
        _keyPad = new KeyPad();
        _dialog = new Dialog();
        _banner = new FloorBanner();
        _gm = new GameManager();

        _player = _gm.CreatePlayer(2, 5);

        var enemyLayer = new GameObject(LayerName.Enemy);

        bool b = true;
        if (b) {
            _enemies.Add(EnemyFactory.CreateEnemy(1, 4, enemyLayer));
            _enemies.Add(EnemyFactory.CreateEnemy(2, 2, enemyLayer));

            _enemies.Add(EnemyFactory.CreateEnemy(1, 5, enemyLayer));
            _enemies.Add(EnemyFactory.CreateEnemy(1, 6, enemyLayer));
            _enemies.Add(EnemyFactory.CreateEnemy(1, 7, enemyLayer));
            _enemies.Add(EnemyFactory.CreateEnemy(1, 8, enemyLayer));
            _enemies.Add(EnemyFactory.CreateEnemy(1, 9, enemyLayer));

            foreach (var e in _enemies) e.AddStatus(Status.Sleep);
        }

        // カメラズーム
        var camera = GameObject.Find("Main Camera");
        camera.GetComponent<Camera>().orthographicSize = _cameraManager.CurrentSize;
        var pos = _player.Position;
        float z = camera.transform.position.z;
        camera.transform.position = new Vector3(pos.x, pos.y + Config.CameraOffsetY, z);

        _floor = FloorCreator.CreateFloor(1);
        ChangeGameState(GameState.TurnStart);

        // Zoom ボタンのクリックイベント
        var btn = GameObject.Find("Button_Zoom").GetComponent<Button>();
        btn.onClick.AddListener(() => {
            if (_gameState == GameState.InputWait) {
                camera.GetComponent<Camera>().orthographicSize = _cameraManager.NextSize();
            }
        });

        var btn2 = GameObject.Find("Canvas/Button_Skill").GetComponent<Button>();
        btn2.onClick.AddListener(() => {
            if (_gameState == GameState.InputWait) {
                ExecutePlayerUseSkill();
            }
        });

        _player.SyncCameraPosition();
    }

    void Update() {
        _floor.UpdateMinimapPlayerIconBlink();

        if (_gameState == GameState.Act) {
            UpdateAct();
            return;
        }

        Assert.IsTrue(_gameState == GameState.InputWait);
        CheckInput();
    }

    void OnGUI() {
        // Func<string, bool> button = (caption) => {
        //     return GUILayout.Button(caption, GUILayout.Width(110), GUILayout.Height(50));
        // };

        int x = 300;
        if (GUI.Button(new Rect(x, 0, 100, 40), "test")) {
            StartCoroutine(NextFloor());
        }
        else if (GUI.Button(new Rect(x, 40*1, 100, 40), "test2")) {
        }
        else if (GUI.Button(new Rect(x, 40*2, 100, 40), "zoom in")) {
            ZoomIn();
        }
        else if (GUI.Button(new Rect(x, 40*3, 100, 40), "zoom out")) {
            ZoomOut();
        }
    }

    private void CheckInput() {
        if (_dialog.IsOpen) return;

        Dir dir;
        if (_keyPad.IsMove(out dir)) {
            int drow = 0;
            int dcol = 0;
            switch (dir) {
            case Dir.N:  drow = -1; dcol =  0; break;
            case Dir.NE: drow = -1; dcol =  1; break;
            case Dir.E:  drow =  0; dcol =  1; break;
            case Dir.SE: drow =  1; dcol =  1; break;
            case Dir.S:  drow =  1; dcol =  0; break;
            case Dir.SW: drow =  1; dcol = -1; break;
            case Dir.W:  drow =  0; dcol = -1; break;
            case Dir.NW: drow = -1; dcol = -1; break;
            }
            ExecutePlayerMove(drow, dcol);
            return;
        }
        else if (_keyPad.IsAttack()) {
            ExecutePlayerAttack();
            return;
        }
        else if (_keyPad.IsFireTrap()) {
            ExecutePlayerFireTrap();
            return;
        }

        if (Input.GetKey(KeyCode.A)) { // 回復アイテム使う
            ExecutePlayerUseItem();
        }
        else if (Input.GetKey(KeyCode.S)) { // スキル使う
            ExecutePlayerUseSkill();
        }
        else if (Input.GetKey(KeyCode.Period)) { // 何もせずターン終了
            ExecutePlayerWait();
        }
    }

    private IEnumerator NextFloor() {
        yield return _banner.FadeInAnimation("ダンジョン名", 1);

        // TODO:次のフロア生成
        yield return new WaitForSeconds(1.6f);

        yield return _banner.FadeOutAnimation();
    }

    private void ZoomIn() {
        var camera = GameObject.Find("Main Camera");
        camera.GetComponent<Camera>().orthographicSize -= 0.1f;
    }

    private void ZoomOut() {
        var camera = GameObject.Find("Main Camera");
        camera.GetComponent<Camera>().orthographicSize += 0.1f;
    }

    public float OrthographicSize {
        get {
            var camera = GameObject.Find("Main Camera");
            return camera.GetComponent<Camera>().orthographicSize;
        }
    }

    public IEnumerator CameraZoom(float delta) {
        var camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        float src = OrthographicSize;
        float dst = src + delta;

        float duration = 0.5f;
        float elapsed = 0;
        while (elapsed <= duration) {
            elapsed += Time.deltaTime;
            float size = UTween.Ease(EaseType.OutCubic, src, dst, elapsed / duration);
            camera.orthographicSize = size;
            yield return null;
        }
        camera.orthographicSize = dst;
    }

    public IEnumerator CameraZoomIn(float delta) {
        return CameraZoom(-delta);
    }

    public IEnumerator CameraZoomOut(float delta) {
        return CameraZoom(delta);
    }

    private void ChangeGameState(GameState nextGameState) {
        _gameState = nextGameState;

        switch (nextGameState) {
        case GameState.Act:
            Debug.Log("### Act ");
            break;

        case GameState.InputWait:
            Debug.Log("### Input Wait ");
            CheckInput();
            break;

        case GameState.TurnStart:
            Debug.Log("### Turn Start ");
            SysTurnStart();

            _player.HideDirection();

            ChangeGameState(GameState.InputWait);
            break;

        case GameState.TurnEnd:
            Debug.Log("### Turn Finish ");
            SysTurnEnd();

            ChangeGameState(GameState.TurnStart);
            break;

        default:
            Assert.IsTrue(false);
            break;
        }
    }

    private void UpdateAct() {
        Assert.IsTrue(_player.Hp > 0);

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

        if (!moveFinished) return; // 移動タスクが完了するまで待機

        bool trapFinished = true;
        foreach (var act in _acts) {
            if (act.IsTrapAct() && !act.Finished) {
                act.UpdateAct(this);
                trapFinished = trapFinished && act.Finished;
            }
        }

        if (!trapFinished) return; // トラップタスクが完了するまで待機

        // 移動タスク、トラップタスクが完了したら、それ以外の行動を一つずつ実行していく
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
            if (acts.Count == 0) { // 全てのキャラの行動が終了した
                ChangeGameState(GameState.TurnEnd);
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

        _floor.UpdateMinimap(_player.Loc, _enemies);

        _player.OnTurnStart();
        foreach (var e in _enemies) {
            e.OnTurnStart();
        }
    }

    // ターン終了後
    private void SysTurnEnd() {
        _acts.Clear();

        var text = GameObject.Find("Canvas/Text").GetComponent<Text>();
        text.text = DLog.ToText();
        DLog.Clear();

        _player.OnTurnEnd();
        foreach (var e in _enemies) {
            e.OnTurnEnd();
        }
    }

    // プレイヤーの行動
    // TODO: 引数を Dir に変更
    private void ExecutePlayerMove(int drow, int dcol) {
        Assert.IsTrue(_acts.Count == 0);

        Dir dir = Utils.ToDir(drow, dcol);
        Loc to = _player.Loc.Forward(dir);
        if (_floor.CanAdvance(_player.Loc, dir) && !ExistsEnemy(to)) {
            _player.ShowDirection(dir);
            _acts.Add(new ActPlayerMove(_player, drow, dcol));

            // TODO:移動先にトラップがあるなら、トラップイベントを発生させる
            Trap trap = _floor.FindTrap(to);
            if (trap != null) {
                _acts.Add(new ActTrap(_player, trap));
            }

            _acts.AddRange(DetectEnemyAct(to));
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

        Enemy enemy = FindEnemy(loc);
        if (enemy != null && _floor.CanAdvance(_player.Loc, dir)) {
            // 敵へ攻撃する
            _acts.Add(new ActPlayerAttack(_player, enemy));
            ChangeGameState(GameState.Act);
            return;
        }

        Treasure treasure = _floor.FindTreasure(loc);
        if (treasure != null) {
            // 宝箱を開ける
            _acts.Add(new ActPlayerOpenTreasure(_player, treasure));
            ChangeGameState(GameState.Act);
            return;
        }

        NoticeBoard noticeBoard = _floor.FindNoticeBoard(loc);
        if (noticeBoard != null) {
            // 立て札を読む場合はターンを消費しない
            ShowDialog(noticeBoard.Msg);
            return;
        }

        ExecutePlayerWait();
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

    private void ExecutePlayerFireTrap() {
        Assert.IsTrue(_acts.Count == 0);

        Trap trap = _floor.FindTrap(_player.Loc);
        if (trap == null) {
            ExecutePlayerWait();
        }
        else {
            _acts.Add(new ActTrap(_player, trap));
            ChangeGameState(GameState.Act);
        }
    }

    private List<Act> DetectEnemyAct(Loc playerNextLoc) {
        return EnemyStrategy.Detect(_enemies, _player, playerNextLoc, _floor);
    }

    private Enemy FindEnemy(Loc loc) {
        foreach (var e in _enemies) {
            if (loc == e.Loc) {
                return e;
            }
        }
        return null;
    }

    public bool ExistsEnemy(Loc loc) {
        return FindEnemy(loc) != null;
    }

    public IEnumerator Summon(Loc loc) {
        Assert.IsTrue(false);
        yield break;

        // TODO: assert(loc に敵がいない)
        // var e = EnemyFactory.CreateEnemy(loc.Row, loc.Col);
        // _enemies.Add(e);

        // yield return Anim.Par(this,
        //                       () => e.FadeIn(),
        //                       () => EffectAnim.Aura2(e.Position));
    }

    public void ShowMinimap() {
        _floor.ShowMinimap();
    }

    public void HideMinimap()  {
        _floor.HideMinimap();
    }

    public void ShowDialog(string message) {
        _dialog.Show(message);
    }
}
