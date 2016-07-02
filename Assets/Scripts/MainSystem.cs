﻿using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

enum GameState {
    InputWait, // 入力待ち
    Act,       // 行動中

    TurnStart,  // ターン開始
    TurnEnd, // ターン終了

    ConfirmStairsDialog, // 階段を降りますかダイアログ
    ConfirmGiveup, // ギブアップダイアログ

    NextFloorTransition,

    DisplayItemWindow,
    DisplayFootItemCommandWindow,
    DisplayFootTrapCommandWindow,
    DisplayDialog,

    ChangeDirWaitPress,   // キーが入力されるのを待つ
    ChangeDirWaitRelease, // キー入力が終了するのを待つ(指が離れるのを待つ)
}

public class MainSystem : MonoBehaviour {
    [SerializeField]
    Button _btnItem;

    [SerializeField]
    Button _btnFoot;

    [SerializeField]
    Button _btnChangeDir;

    [SerializeField]
    Button _btnGiveup;

    [SerializeField]
    GameObject _itemWindow;

    [SerializeField]
    GameObject _footItemCommandWindow;

    [SerializeField]
    GameObject _footTrapCommandWindow;

    [SerializeField]
    GameObject _yesNoDialog;

    [SerializeField]
    GameObject _okDialog;

    private CameraManager _cameraManager = new CameraManager();
    private GameState _gameState;
    private Player _player;
    private List<Enemy> _enemies = new List<Enemy>();
    private List<FieldItem> _fieldItems = new List<FieldItem>();

    private List<Act> _acts = new List<Act>();
    private MessageManager _mm;

    private int _turnCount = 0;

    private KeyPad _keyPad;
    private FloorBanner _banner;
    private GameManager _gm;
    private Floor _floor;
    private int _floorNumber = 0;

    private int _gold;

    void Awake() {
        // iOS でかくつく問題の対策
#if UNITY_IOS
        Application.targetFrameRate = 60;
#endif
    }

    void Start() {
        _okDialog.SetActive(false);
        _yesNoDialog.SetActive(false);
        _footItemCommandWindow.SetActive(false);
        _footTrapCommandWindow.SetActive(false);

        LayerManager.CreateAllLayer();

        _itemWindow.SetActive(false);
        _btnItem.onClick.AddListener(() => {
            if (_gameState != GameState.InputWait) return;

            ChangeGameState(GameState.DisplayItemWindow);
            _itemWindow.SetActive(true);

            var sc = GameObject.Find("Canvas/ScrollView/Panel/Content").GetComponent<ScrollController>();
            sc.Init(_player.Items);

            sc.SetCloseCallback(() => {
                ChangeGameState(GameState.InputWait);
            });
            sc.SetItemActionCallback((ItemActionType actionType, Item item) => {
                Debug.Log("callback = " + actionType);
                Debug.Log("item = " + item);

                ExecutePlayerItemAction(actionType, item);
            });
        });

        _btnFoot.onClick.AddListener(() => {
            if (_gameState != GameState.InputWait) return;

            Debug.Log("足下ボタンが押されました");

            // 足下アイテム
            var fieldItem = FindFieldItem(_player.Loc);
            if (fieldItem != null) {
                ChangeGameState(GameState.DisplayFootItemCommandWindow);
                var c = _footItemCommandWindow.GetComponent<FootItemCommandWindow>();
                c.Init(fieldItem, (ItemActionType actionType, FieldItem fitem) => {
                    Debug.LogFormat("--> type:{0} fieldItem:{1}", actionType, fitem);

                    ExecutePlayerFootItemAction(actionType, fitem);
                });
                _footItemCommandWindow.SetActive(true);
                return;
            }

            // 足下ワナ
            var trap = _floor.FindTrap(_player.Loc);
            if (trap != null) {
                ChangeGameState(GameState.DisplayFootTrapCommandWindow);
                var c = _footTrapCommandWindow.GetComponent<FootTrapCommandWindow>();
                c.Init(trap, (TrapActionType actionType, Trap t) => {
                    Debug.LogFormat("--> type:{0} trap:{1}", actionType, t);
                    ExecutePlayerFootTrapAction(actionType, t);
                });

                _footTrapCommandWindow.SetActive(true);
                return;
            }

            // 足下階段
            if (_player.Loc == _floor.StairsLoc) {
                ConfirmDownStairs(GameState.InputWait);
                ChangeGameState(GameState.ConfirmStairsDialog);
                return;
            }
        });

        _btnGiveup.onClick.AddListener(() => {
            if (_gameState != GameState.InputWait) return;
            ChangeGameState(GameState.ConfirmGiveup);
        });

        _btnChangeDir.onClick.AddListener(() => {
            if (_gameState != GameState.InputWait) return;

            Debug.Log("方向転換します");
            _player.ShowDirectionAll();
            ChangeGameState(GameState.ChangeDirWaitPress);
        });

        DLog.Enable = false;
        _keyPad = new KeyPad();
        _banner = new FloorBanner();
        _gm = new GameManager();
        _mm = new MessageManager(this);

        _player = _gm.CreatePlayer(new Loc(3, 3));
        _player.setCallback(OnPlayerStatusAdded, OnPlayerStatusRemoved);

        var camera = GameObject.Find("Main Camera");
        camera.GetComponent<Camera>().orthographicSize = _cameraManager.CurrentSize;

        // Zoom ボタンのクリックイベント
        var btnZoom = GameObject.Find("Button_Zoom");
        btnZoom.GetComponent<Button>().onClick.AddListener(() => {
            if (_gameState == GameState.InputWait) {
                camera.GetComponent<Camera>().orthographicSize = _cameraManager.NextSize();
            }
        });
        btnZoom.SetActive(false);

        ChangeGameState(GameState.NextFloorTransition);
        StartCoroutine(NextFloor(true));
    }

    private void ShowOKDialog(string message, Action ok) {
        ChangeGameState(GameState.DisplayDialog);
        new Dialog(_okDialog, message, ok);
    }

    private void ShowOKDialog(string message) {
        ShowOKDialog(message, () => { ChangeGameState(GameState.InputWait); });
    }

    private void ShowYesNoDialog(string message, Action yes, Action no) {
        ChangeGameState(GameState.DisplayDialog);
        new YesNoDialog(_yesNoDialog, message, yes, no);
    }

    private void ConfirmDownStairs(GameState cancelState) {
        Action yes = () => {
            ChangeGameState(GameState.NextFloorTransition);
            StartCoroutine(NextFloor());
        };
        Action no = () => {
            ChangeGameState(cancelState);
        };
        ShowYesNoDialog("階段をおりますか？", yes, no);
    }

    private void ConfirmGiveup() {
        Action yes = () => {
            _floorNumber--;
            ChangeGameState(GameState.NextFloorTransition);
            StartCoroutine(NextFloor());
        };
        Action no = () => {
            ChangeGameState(GameState.InputWait);
        };
        ShowYesNoDialog("ギブアップしますか？", yes, no);
    }

    private Enemy MakeEnemy(Loc loc, int sleepDepth) {
        var e = EnemyFactory.CreateEnemy(loc);
        if (sleepDepth > 0) {
            e.AddStatus(StatusType.Sleep, sleepDepth);
        }
        return e;
    }

    // フロアに敵を配置する
    private void SetupFloorEnemy(int n, GameObject layer) {
        Assert.IsTrue(_enemies.Count == 0);
        if (DebugConfig.NoEnemy) return;

        Room[] rooms = _floor.GetRooms();
        for (int i = 0; i < n; i++) {
            const int tryCount = 10;
            for (int j = 0; j < tryCount; j++) {
                var loc = Utils.RandomRoomLoc(rooms.Choice());

                // TODO:敵を配置可能か調べるメソッド
                if (_floor.ExistsObstacle(loc)) continue;
                if (_floor.IsWater(loc)) continue;

                if (_enemies.Any(e => e.Loc == loc)) continue;
                if (_player.Loc == loc) continue;

                _enemies.Add(EnemyFactory.CreateEnemy(loc));
                break;
            }
        }
    }

    // フロアにアイテムを配置する
    private void SetupFloorItem(int n, GameObject layer) {
        Assert.IsTrue(_fieldItems.Count == 0);

        var rand = new System.Random();
        Room[] rooms = _floor.GetRooms();
        for (int i = 0; i < n; i++) {
            const int tryCount = 10;
            for (int j = 0; j < tryCount; j++) {
                var loc = Utils.RandomRoomLoc(rooms.Choice());

                // 既にアイテム配置済みなら置けない
                if (_fieldItems.Where(e => e.Loc == loc).Any()) continue;

                if (_floor.CanPutItem(loc)) {
                    if (rand.Next(2) % 2 == 100) {
                        var item = FieldItemFactory.CreateHerb(loc, 0);
                        AddFieldItem(item);
                    }
                    else {
                        //var item = FieldItemFactory.CreateHerb(loc, layer);
                        var item = FieldItemFactory.CreateHerb(loc, 0);
                        AddFieldItem(item);
                    }
                    // AddFieldItem(FieldItemFactory.CreateHerb(loc, layer));
                    break;
                }
            }
        }
    }

    public void AddFieldItem(FieldItem fieldItem) {
        _fieldItems.Add(fieldItem);
    }

    public void AddEnemy(Enemy enemy) {
        _enemies.Add(enemy);
    }

    public void FallItemToFloor(Loc fallLoc, FieldItem fieldItem) {
        // TODO:投げたアイテムが落ちる場所は、プレイヤーに近い位置から優先的に決める
        var locs = new List<Loc>();
        int n = 3;
        for (int i = -n; i <= n; i++) {
            for (int j = -n; j <= n; j++) {
                locs.Add(new Loc(fallLoc.Row + i, fallLoc.Col + j));
            }
        }

        locs.Sort((a, b) => {
            int s = a.SquareDistance(fallLoc);
            int t = b.SquareDistance(fallLoc);
            if (s == t) {
                if (a.Row == b.Row) return a.Col.CompareTo(b.Col);
                return a.Row.CompareTo(b.Row);
            }
            return s.CompareTo(t);
        });

        bool put = false;
        foreach (var loc in locs) {
            if (ExistsFieldItem(loc)) continue;

            if (_floor.CanPutItem(loc)) {
                fieldItem.UpdateLoc(loc);
                AddFieldItem(fieldItem);
                put = true;
                break;
            }
        }

        if (!put) {
            Debug.Log("アイテムは配置できなかった");
            fieldItem.Destroy();
        }
    }

    void Update() {
        _mm.Update();

        if (_gameState == GameState.NextFloorTransition) return;

        _floor.UpdateMinimapPlayerIconBlink();

        switch (_gameState) {
        case GameState.DisplayDialog:
        case GameState.DisplayItemWindow:
        case GameState.DisplayFootItemCommandWindow:
        case GameState.DisplayFootTrapCommandWindow:
        case GameState.ConfirmStairsDialog:
        case GameState.ConfirmGiveup:
            return;
        }

        if (_gameState == GameState.ChangeDirWaitPress) {
            Dir dir;
            if (_keyPad.IsMove(out dir)) {
                _player.ChangeDir(dir);
                ChangeGameState(GameState.ChangeDirWaitRelease);
                return;
            }
            return;
        }
        else if (_gameState == GameState.ChangeDirWaitRelease) {
            Dir dir;
            if (_keyPad.IsMove(out dir)) {
                // ボタンが押された状態
            }
            else {
                _player.HideDirection();
                ChangeGameState(GameState.InputWait);
            }
            return;
        }

        if (_gameState == GameState.Act) {
            UpdateAct();
            return;
        }
        Assert.IsTrue(_gameState == GameState.InputWait);
        CheckInput();
    }

/*
    void OnGUI() {
        int x = 300;
        if (GUI.Button(new Rect(x, 0, 100, 40), "test")) {
            StartCoroutine(NextFloor());
        }
        else if (GUI.Button(new Rect(x, 40*1, 100, 40), "test2")) {
            // StartCoroutine(Test());
        }
    }
*/

    private void CheckInput() {
        Dir dir;
        if (_keyPad.IsMove(out dir)) {
            ExecutePlayerMove(dir);
            return;
        }
        else if (_keyPad.IsAttack()) {
            ExecutePlayerAttack();
            return;
        }

        if (Input.GetKey(KeyCode.A)) {
            foreach (var e in _enemies) {
                e.AddStatus(StatusType.Sleep);
            }
        }
        else if (Input.GetKey(KeyCode.Period)) { // 何もせずターン終了
            ExecutePlayerWait();
        }
    }

    private IEnumerator NextFloor(bool isSkipTransition = false) {
        ++_floorNumber;
        if (_floorNumber == 10) _floorNumber = 1;
        if (!isSkipTransition) {
            yield return _banner.FadeInAnimation("テストダンジョン", _floorNumber);
        }

        _enemies.Clear();
        _fieldItems.Clear();

        LayerManager.RemoveAllLayer();
        // 同一フレームで、同じ GameObject に対して Destroy したあとに new GameObject できないようなので
        // yield return null で生成のタイミングをずらす
        yield return null;
        LayerManager.CreateAllLayer();

        _floor = FloorCreator.CreateFloor(_floorNumber, _player, this);
        yield return null; // TODO:yield return null を入れるとミニマップの位置が更新される
        _player.SyncCameraPosition();
        UpdatePassageSpotlightPosition(_player.Position);
        UpdateSpot(_player.Loc);

        var room = FindRoom(_player.Loc);
        if (room != null) {
            OnRoomEntering(room, _player.Loc);
            OnRoomEntered(room, _player.Loc);
        }

        var text = GameObject.Find("Canvas/Header/Text_Floor").GetComponent<Text>();
        text.text = string.Format("{0}F", _floorNumber);

        if (!isSkipTransition) {
            yield return new WaitForSeconds(0.7f);
            yield return _banner.FadeOutAnimation();
        }
        ChangeGameState(GameState.TurnStart);
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

            // TODO:直前の行動が移動のみ階段ダイアログを表示する
            // 階段の上ならダイアログ表示
            if (_player.Loc == _floor.StairsLoc) {
                _player.HideDirection();
                ConfirmDownStairs(GameState.TurnStart);
                ChangeGameState(GameState.ConfirmStairsDialog);
            }
            else {
                ChangeGameState(GameState.TurnStart);
            }
            break;

        case GameState.ConfirmStairsDialog:
            break;

        case GameState.ConfirmGiveup:
            ConfirmGiveup();
            break;

        case GameState.NextFloorTransition:
            break;

        case GameState.DisplayItemWindow:
            break;

        case GameState.DisplayFootItemCommandWindow:
            break;

        case GameState.DisplayFootTrapCommandWindow:
            break;

        case GameState.DisplayDialog:
            break;

        case GameState.ChangeDirWaitPress:
            break;

        case GameState.ChangeDirWaitRelease:
            break;

        default:
            Assert.IsTrue(false);
            break;
        }
    }

    private void UpdateAct() {
        if (_player.Hp <= 0) {
            // ゲームオーバーダイアログを表示して、フロアをやり直す。
            Action ok = () => {
                _acts.Clear();
                _floorNumber--;
                ChangeGameState(GameState.NextFloorTransition);
                StartCoroutine(NextFloor());
            };
            ShowOKDialog("ゲームオーバーです。\nフロアをやり直します。", ok);
            return;
        }

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
        // TODO: act.IsInvalid() で無効になったキャラがいるなら、
        //       ただちに Detect で行動順を決めないと、攻撃の後に移動処理がくることになるはず。
        bool actFinished = true;
        foreach (var act in _acts) {
            if (act.Actor.Hp <= 0 || act.IsInvalid()) continue;
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

        UpdateMinimap();

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

    public void UpdateMinimap() {
        _floor.UpdateMinimapIconAll(_player.Loc, _enemies, _fieldItems);
    }

    // プレイヤーの行動
    private void ExecutePlayerMove(Dir dir) {
        Assert.IsTrue(_acts.Count == 0);

        Loc to = _player.Loc.Forward(dir);
        if (_floor.CanAdvance(_player.Loc, dir) && !ExistsEnemy(to)) {
            _player.ShowDirection(dir);
            _acts.Add(new ActPlayerMove(_player, dir, FindFieldItem(to)));

            // 移動先にトラップがあるなら、トラップイベントを発生させる
            Trap trap = _floor.FindTrap(to);
            if (trap != null) {
                // TODO:Fire確率
                _acts.Add(new ActTrap(_player, trap));
            }

            _acts.AddRange(DetectEnemyAct(to));
            ChangeGameState(GameState.Act);
        }
        else {
            // Debug.Log("進めません");
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
            ShowOKDialog(noticeBoard.Msg);
            return;
        }

        ExecutePlayerWait();
    }

    private void ExecutePlayerItemAction(ItemActionType actionType, Item item) {
        Debug.Log("Count = " + _acts.Count);
        Assert.IsTrue(_acts.Count == 0);

        switch (actionType) {
        case ItemActionType.Use:
            ExecutePlayerUseItem(item);
            break;
        case ItemActionType.Throw:
            ExecutePlayerThrowItem(item);
            break;
        case ItemActionType.Put:
            ExecutePlayerPutItem(item);
            break;
        default:
            Assert.IsTrue(false);
            break;
        }
    }

    private void ExecutePlayerFootItemAction(ItemActionType actionType, FieldItem fieldItem) {
        Assert.IsTrue(_acts.Count == 0);

        switch (actionType) {
        case ItemActionType.Close:
            ChangeGameState(GameState.InputWait);
            break;
        case ItemActionType.Use:
            ExecutePlayerUseFootItem(fieldItem);
            break;
        case ItemActionType.Throw:
            ExecutePlayerThrowFootItem(fieldItem);
            break;
        case ItemActionType.Take:
            ExecutePlayerTakeFootItem(fieldItem);
            break;
        default:
            Assert.IsTrue(false);
            break;
        }
    }

    private void ExecutePlayerFootTrapAction(TrapActionType actionType, Trap trap) {
        Assert.IsTrue(_acts.Count == 0);

        switch (actionType) {
        case TrapActionType.Close:
            ChangeGameState(GameState.InputWait);
            break;
        case TrapActionType.Fire:
            _acts.Add(new ActTrap(_player, trap));
            ChangeGameState(GameState.Act);
            break;
        default:
            Assert.IsTrue(false);
            break;
        }
    }

    private void ExecutePlayerUseItem(Item item) {
        Debug.Log("---- Use Item");
        Assert.IsTrue(_acts.Count == 0);

        if (item.Type == ItemType.Wand) {
            _acts.Add(new ActPlayerUseWand(_player, item));
        }
        else {
            _acts.Add(new ActPlayerUseItem(_player, item));
        }

        ChangeGameState(GameState.Act);
    }

    private void ExecutePlayerUseFootItem(FieldItem fieldItem) {
        Assert.IsTrue(_acts.Count == 0);

        Item item = fieldItem.Item;
        if (item.Type == ItemType.Wand) {
            _acts.Add(new ActPlayerUseWand(_player, item));
        }
        else {
            _acts.Add(new ActPlayerUseFootItem(_player, fieldItem));
        }
        ChangeGameState(GameState.Act);
    }

    private void ExecutePlayerPutItem(Item item) {
        Debug.Log("--- Put Item");
        Assert.IsTrue(_acts.Count == 0);

        bool ok = true;
        if (!_floor.CanPutItem(_player.Loc)) {
            ok = false;
        }
        // 足下に既にアイテムがある
        if (_fieldItems.Where(e => e.Loc == _player.Loc).Any()) {
            ok = false;
        }

        if (ok) {
            _acts.Add(new ActPlayerPutItem(_player, item));
            ChangeGameState(GameState.Act);
        }
        else {
            Debug.Log("ここにアイテムは置けません");
            ChangeGameState(GameState.InputWait);
        }
    }

    private void ExecutePlayerTakeFootItem(FieldItem fieldItem) {
        Assert.IsTrue(_acts.Count == 0);

        // TODO:アイテムが持ちきれない場合
        _acts.Add(new ActPlayerTakeFootItem(_player, fieldItem));
        ChangeGameState(GameState.Act);
    }

    private void ExecutePlayerThrowItem(Item item) {
        // TODO:石を投げる場合は別処理
        Assert.IsTrue(_acts.Count == 0);

        Loc loc = _player.Loc;
        bool update = true;
        while (update) {
            update = false;

            var next = loc.Forward(_player.Dir);
            if (_floor.IsWall(next) || _floor.ExistsObstacle(next)) {
                _acts.Add(new ActPlayerThrowItem(_player, item, next, loc, null));
            }
            else {
                int p = _enemies.FindIndex(e => e.Loc == next);
                if (p != -1) { // 敵にヒット
                    var target = _enemies[p];
                    _acts.Add(new ActPlayerThrowItem(_player, item, next, next, target));
                }
                else {
                    update = true;
                    loc = next;
                }
            }
        }
        ChangeGameState(GameState.Act);
    }

    private void ExecutePlayerThrowFootItem(FieldItem fieldItem) {
        // TODO:石を投げる場合は別処理
        Assert.IsTrue(_acts.Count == 0);

        Loc loc = _player.Loc;
        bool update = true;
        while (update) {
            update = false;

            var next = loc.Forward(_player.Dir);
            if (_floor.IsWall(next) || _floor.ExistsObstacle(next)) {
                _acts.Add(new ActPlayerThrowFootItem(_player, fieldItem, next, loc, null));
            }
            else {
                int p = _enemies.FindIndex(e => e.Loc == next);
                if (p != -1) { // 敵にヒット
                    var target = _enemies[p];
                    _acts.Add(new ActPlayerThrowFootItem(_player, fieldItem, next, next, target));
                }
                else {
                    update = true;
                    loc = next;
                }
            }
        }
        ChangeGameState(GameState.Act);
    }

    public Loc FindHitTarget(Loc src, Dir front, out CharacterBase hitTarget) {
        hitTarget = null;

        Loc loc = src;
        bool update = true;
        while (update){
            update = false;

            var next = loc.Forward(front);
            if (_floor.IsWall(next) || _floor.ExistsObstacle(next)) {
                return next;
            }
            else {
                int p = _enemies.FindIndex(e => e.Loc == next);
                if (p != -1) {
                    hitTarget = _enemies[p];
                    return next;
                }
                update = true;
                loc = next;
            }
        }

        Assert.IsTrue(false);
        return Loc.Zero;
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

    private FieldItem FindFieldItem(Loc loc) {
        for (int i = 0; i < _fieldItems.Count; i++) {
            if (_fieldItems[i].Loc == loc) {
                return _fieldItems[i];
            }
        }
        return null;
    }

    private bool ExistsFieldItem(Loc loc) {
        return FindFieldItem(loc) != null;
    }

    public void RemoveFieldItem(FieldItem fieldItem) {
        for (int i = 0; i < _fieldItems.Count; i++) {
            if (_fieldItems[i].Loc == fieldItem.Loc) {
                Assert.IsTrue(System.Object.ReferenceEquals(_fieldItems[i], fieldItem));
                fieldItem.Destroy();
                _fieldItems.RemoveAt(i);
                return;
            }
        }
        Assert.IsTrue(false);
    }

    public IEnumerator Summon(Loc loc) {
        // TODO: assert(loc に敵がいない)
        var e = EnemyFactory.CreateEnemy(loc);
        _enemies.Add(e);

        yield return Anim.Par(this,
                              () => e.FadeIn(),
                              () => EffectAnim.Aura2(e.Position));
    }

    public void ShowMinimap() {
        _floor.ShowMinimap();
    }

    public void HideMinimap()  {
        _floor.HideMinimap();
    }

    // TODO: RandomWarpLoc
    // TODO:defaultLoc 削除。部屋のランダムなLocを返す。
    // 部屋内のランダムな位置を返す
    // ただし以下が存在する場合は除く
    // - 敵
    // - 水
    // - 障害物
    public Loc RandomRoomLoc(Loc defaultLoc) {
        var rand = new System.Random();
        const int retryCount = 20;
        for (int i = 0; i < retryCount; i++) {
            var room = _floor.GetRooms().Choice();

            int r = rand.Next(room.Row, room.Row + room.Height);
            int c = rand.Next(room.Col, room.Col + room.Width);
            var loc = new Loc(r, c);
            if (_floor.IsWater(loc)) continue;
            if (_floor.ExistsObstacle(loc)) continue;
            if (!_enemies.Any(e => e.Loc == loc)) {
                return loc;
            }
        }
        return defaultLoc; // ワープできない場合
    }


    public Loc Warp(Loc source, bool excludeSourceRoom) {
        const int retryCount = 20;
        Room[] rooms = _floor.GetRooms();
        if (excludeSourceRoom && rooms.Length > 1) {
            rooms = rooms.Where(e => !e.IsInside(source)).ToArray();
        }

        for (int i = 0; i < retryCount; i++) {
            var room = rooms.Choice();

            int r = Rand.Next(room.Row, room.EndRow + 1);
            int c = Rand.Next(room.Col, room.EndCol + 1);
            var loc = new Loc(r, c);
            if (_floor.IsWater(loc)) continue;
            if (_floor.ExistsObstacle(loc)) continue;
            if (!_enemies.Any(e => e.Loc == loc)) {
                return loc;
            }
        }
        return source; // ワープできる場所がないなら source にワープ
    }

    public void IncGold(int gold) {
        _gold += gold;

        var text = GameObject.Find("Canvas/Header/Text_G_Value").GetComponent<Text>();
        text.text = _gold.ToString();
    }

    // 水がれ
    public IEnumerator Sun(Loc playerLoc) {
        yield return _floor.Sun(playerLoc);
    }

    public CharacterBase[] GetEnemies() {
        return _enemies.ToArray();
    }

    public Enemy[] CollectNeighborEnemies(Loc loc) {
        var enemies = new List<Enemy>();
        foreach (var loc2 in loc.Neighbors()) {
            var e = FindEnemy(loc2);
            if (e != null) {
                enemies.Add(e);
            }
        }
        return enemies.ToArray();
    }

    // めぐすり
    public void Eyedrops() {
        _player.AddStatus(StatusType.VisibleAll, 3);
        _floor.Eyedrops();
    }

    public bool IsWater(Loc loc) {
        return _floor.IsWater(loc);
    }

    public void Message(string msg) {
        _mm.Message(msg);
    }

    public void Message(string msg1, string msg2) {
        _mm.Message(msg1, msg2);
    }

    public void Msg_UseItem(Item item) {
        string doing = "使った！";
        switch (item.Type) {
        default:
        case ItemType.Herb:
        case ItemType.Gold:
            doing = "使った！";
            break;
        case ItemType.Magic:
            doing = "読んだ！";
            break;
        case ItemType.Wand:
            doing = "振った！";
            break;
        }
        _mm.Message(string.Format("{0} を{1}", item.Name, doing));
    }

    public void Msg_ThrowItem(Item item) {
        _mm.Message(string.Format("{0} を{1}", item.Name, "投げた！"));
    }

    public void Msg_TakeItem(Item item) {
        _mm.Message(string.Format("{0} を{1}", item.Name, "ひろった"));
    }

    public void UpdateSpot(Loc loc) {
        _floor.UpdateSpot(loc);
    }

    public void UpdatePassageSpotlightPosition(Vector3 pos) {
        _floor.UpdatePassageSpotlightPosition(pos);
    }

    public Room FindRoom(Loc loc) {
        return _floor.FindRoom(loc);
    }

    // モンスターの Visible を更新する
    // スポットライトのあるフロアの移動時や、目薬、めつぶしステータス付与/解除時に呼ばれる
    private void UpdateEnemyVisible(Room room, Loc playerLoc) {
        if (_player.IsVisibleAll()) {
            foreach (var enemy in _enemies) {
                enemy.Visible = true;
            }
        }
        else {
            bool isBlind = _player.IsBlind();
            if (room == null) { // プレイヤーは通路上
                foreach (var enemy in _enemies) {
                    enemy.Visible = !isBlind && playerLoc.IsNeighbor(enemy.NextLoc);
                }
            }
            else { // プレイヤーは部屋内
                foreach (var enemy in _enemies) {
                    enemy.Visible = !isBlind && room.IsInside(enemy.NextLoc, true);
                }
            }
        }
        _floor.UpdateMinimapEnemyIcon(_enemies);
    }

    private void UpdateFieldItemVisible() {
        bool isVisibleAll = _player.IsVisibleAll();
        foreach (var item in _fieldItems) {
            if (isVisibleAll) {
                item.Visible = true;
            }
            else {
                item.ResetVisible();
            }
        }
    }

    public void OnRoomEntering(Room room, Loc playerNextLoc) {
        Debug.Log("部屋に入りました (入る直前)");

        // これまで見えていなかったアイテムを表示する
        foreach (var item in _fieldItems) {
            if (room.IsInside(item.Loc)) {
                item.OnDiscovered(_player.IsBlind());
            }
        }

        // 視界内のモンスターのみ表示する
        UpdateEnemyVisible(room, playerNextLoc);
    }

    public void OnRoomEntered(Room room, Loc playerLoc) {
        Debug.Log("部屋に入りました (入った直後)");

        // モンスターハウスイベント
    }

    public void OnRoomExiting(Room room, Loc playerNextLoc) {
        Debug.Log("部屋から出ました (出る直前)");

        // 視界内のモンスターのみ表示する
        UpdateEnemyVisible(null, playerNextLoc);
    }

    public void OnRoomExited(Room room, Loc playerLoc) {
        Debug.Log("部屋から出ました (出た直後)");
    }

    public void OnRoomMoving(Room room, Loc playerNextLoc) {
        // 視界内のモンスターのみ表示する
        UpdateEnemyVisible(room, playerNextLoc);
    }

    public void OnRoomMoved(Room room, Loc playerNextLoc) {

    }

    public void OnPassageMoving(Loc playerNextLoc) {
        // 視界内のモンスターのみ表示する
        UpdateEnemyVisible(null, playerNextLoc);
    }

    public void OnPassageMoved(Loc playerLoc) {

    }

    public void OnPlayerStatusAdded(StatusType statusType) {
        switch (statusType) {
        case StatusType.VisibleAll:
            UpdateEnemyVisible(FindRoom(_player.Loc), _player.Loc);
            UpdateFieldItemVisible();
            break;
        }
    }

    public void OnPlayerStatusRemoved(StatusType statusType) {
        switch (statusType) {
        case StatusType.VisibleAll:
            UpdateEnemyVisible(FindRoom(_player.Loc), _player.Loc);
            UpdateFieldItemVisible();
            break;
        }
    }
}
