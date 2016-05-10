using UnityEngine;
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
    NextFloorTransition,

    DisplayItemWindow,
}

public class MainSystem : MonoBehaviour {
    [SerializeField]
    Button _btnItem;

    [SerializeField]
    GameObject _itemWindow;

    private CameraManager _cameraManager = new CameraManager();
    private GameState _gameState;
    private Player _player;
    private List<Enemy> _enemies = new List<Enemy>();
    private List<FieldItem> _fieldItems = new List<FieldItem>();

    private List<Act> _acts = new List<Act>();

    private Dialog _dialog;
    private YesNoDialog _yesNoDialog;

    private int _turnCount = 0;

    private KeyPad _keyPad;
    private FloorBanner _banner;
    private GameManager _gm;
    private Floor _floor;

    private int _gold;

    void Start() {
        _itemWindow.SetActive(false);
        _btnItem.onClick.AddListener(() => {
            if (_gameState != GameState.InputWait) return;

            ChangeGameState(GameState.DisplayItemWindow);

            _itemWindow.SetActive(true);

            var sc = GameObject.Find("Canvas/ScrollView/Panel/Content").GetComponent<ScrollController>();
/*
            var items = new[] {
                ItemFactory.CreateHerb(),
                ItemFactory.CreateStone(),
                ItemFactory.CreateMagic(),
            };
            sc.Init(items.ToList());
            */

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

        DLog.Enable = false;
        _keyPad = new KeyPad();
        _dialog = new Dialog();
        _yesNoDialog = new YesNoDialog();
        _banner = new FloorBanner();
        _gm = new GameManager();

        _player = _gm.CreatePlayer(new Loc(3, 5));
        // _player.AddStatus(Status.Invisible);
        _floor = FloorCreator.CreateFloor(1);

        var enemyLayer = new GameObject(LayerName.Enemy);
        SetupFloorEnemy(1, enemyLayer);

        var itemLayer = new GameObject(LayerName.Item);
        SetupFloorItem(5, itemLayer);

        var camera = GameObject.Find("Main Camera");
        camera.GetComponent<Camera>().orthographicSize = _cameraManager.CurrentSize;

        // Zoom ボタンのクリックイベント
        var btn = GameObject.Find("Button_Zoom").GetComponent<Button>();
        btn.onClick.AddListener(() => {
            if (_gameState == GameState.InputWait) {
                camera.GetComponent<Camera>().orthographicSize = _cameraManager.NextSize();
            }
        });

        _player.SyncCameraPosition();
        ChangeGameState(GameState.TurnStart);
    }

    private void SetupFloorEnemy(int n, GameObject layer) {
        Assert.IsTrue(_enemies.Count == 0);
        if (DebugConfig.NoEnemy) return;

        Room[] rooms = _floor.GetRooms();
        for (int i = 0; i < n; i++) {
            const int tryCount = 10;
            for (int j = 0; j < tryCount; j++) {
                var loc = Utils.RandomRoomLoc(Utils.Choice(rooms));

                if (_floor.ExistsObstacle(loc)) continue;
                if (_enemies.Any(e => e.Loc == loc)) continue;
                if (_player.Loc == loc) continue;

                _enemies.Add(EnemyFactory.CreateEnemy(loc, layer));
                break;
            }
        }
    }

    private void SetupFloorItem(int n, GameObject layer) {
        Assert.IsTrue(_fieldItems.Count == 0);

        var rand = new System.Random();
        Room[] rooms = _floor.GetRooms();
        for (int i = 0; i < n; i++) {
            const int tryCount = 10;
            for (int j = 0; j < tryCount; j++) {
                var loc = Utils.RandomRoomLoc(Utils.Choice(rooms));

                // 既にアイテム配置済みなら置けない
                if (_fieldItems.Where(e => e.Loc == loc).Any()) continue;

                if (_floor.CanPutItem(loc)) {
                    if (rand.Next(2) % 2 == 0) {
                        var item = FieldItemFactory.CreateWand(loc, layer);
                        AddFieldItem(item);
                    }
                    else {
                        var item = FieldItemFactory.CreateHerb(loc, layer);
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
        if (_gameState == GameState.NextFloorTransition) return;

        _floor.UpdateMinimapPlayerIconBlink();

        if (_gameState == GameState.DisplayItemWindow) return;

        if (_gameState == GameState.ConfirmStairsDialog) {
            if (_yesNoDialog.IsYesPressed) {
                ChangeGameState(GameState.NextFloorTransition);
                StartCoroutine(NextFloor());
            }
            else if (_yesNoDialog.IsNoPressed) {
                ChangeGameState(GameState.TurnStart);
            }
            return;
        }
        else if (_gameState == GameState.Act) {
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
            // StartCoroutine(Test());
        }
    }

    private void CheckInput() {
        if (_dialog.IsOpen) return;

        Dir dir;
        if (_keyPad.IsMove(out dir)) {
            ExecutePlayerMove(dir);
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
            // ExecutePlayerUseItem();
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
        _enemies.Clear();
        _fieldItems.Clear();

        Destroy(GameObject.Find(LayerName.Enemy));
        Destroy(GameObject.Find(LayerName.Item));
        Destroy(GameObject.Find(LayerName.FieldObject));
        Destroy(GameObject.Find(LayerName.Trap));
        Destroy(GameObject.Find(LayerName.Map));
        Destroy(GameObject.Find(LayerName.Minimap));

        // TODO:フロア遷移でアイテムレイヤーが作られていない
        // フロア遷移直後に全てのレイヤーを作成する

        _floor = FloorCreator.CreateFloor(2);

        // TODO:レイヤー管理
        var enemyLayer = new GameObject(LayerName.Enemy);
        //DeployEnemy(3, enemyLayer);

        _player.UpdateLoc(new Loc(2, 8));
        _player.ChangeDir(Dir.S);
        yield return null; // TODO:yield return null を入れるとミニマップの位置が更新される
        _player.SyncCameraPosition(); // TODO:ミニマップの位置が更新されない

        yield return new WaitForSeconds(1.1f);
        yield return _banner.FadeOutAnimation();

        ChangeGameState(GameState.TurnStart);
    }

    private List<GameObject> D(int n) {
        string pathPrefix = "Prefabs/Digits/digits_green_";

        var pos = _player.Position;
        float fontWidth = 0.14f;
        var digits = new List<GameObject>();
        var ds = Utils.Digits(n);
        float x = pos.x - fontWidth * ds.Length / 2.0f + fontWidth / 2;
        foreach (var d in ds) {
            var obj = Resources.Load(pathPrefix + d);
            var gobj = (GameObject)GameObject.Instantiate(obj, new Vector3(x, pos.y, pos.z), Quaternion.identity);
            digits.Add(gobj);
            x += fontWidth;
        }
        return digits;
    }

    private IEnumerator Test() {
        int fm = 0;
        int to = 35;

        List<GameObject> digits = null;

        float duration = 0.33f;
        float elapsed = 0;
        // カウントアップ
        while (elapsed <= duration) {
            elapsed += Time.deltaTime;
            int x = (int)UTween.Ease(EaseType.OutQuad, fm, to, elapsed / duration);
            if (digits != null) {
                foreach (var d in digits) Destroy(d);
            }
            digits = D(x);
            yield return null;
        }
        yield return new WaitForSeconds(0.4f);

        elapsed = 0;
        duration = 0.32f;

        var src = _player.Position;
        var dst = _player.Position + new Vector3(0, 0.23f, 0);

        while (elapsed <= duration) {
            elapsed += Time.deltaTime;
            float y = Mathf.Lerp(src.y, dst.y, elapsed / duration);
            foreach (var d in digits) {
                var p = d.transform.position;
                d.transform.position = new Vector3(p.x, y, p.z);

                var renderer = d.GetComponent<SpriteRenderer>();
                var color = renderer.color;
                var a = UTween.Ease(EaseType.InQuint, 1.0f, 0, elapsed / duration);
                color.a = a;
                renderer.color = color;

            }
            yield return null;
        }

        foreach (var d in digits) {
            Destroy(d);
        }
    }

    // private void ZoomIn() {
    //     var camera = GameObject.Find("Main Camera");
    //     camera.GetComponent<Camera>().orthographicSize -= 0.1f;
    // }

    // private void ZoomOut() {
    //     var camera = GameObject.Find("Main Camera");
    //     camera.GetComponent<Camera>().orthographicSize += 0.1f;
    // }

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
                ChangeGameState(GameState.ConfirmStairsDialog);
            }
            else {
                ChangeGameState(GameState.TurnStart);
            }
            break;

        case GameState.ConfirmStairsDialog:
            _yesNoDialog.Show("階段を降りますか？");
            break;

        case GameState.NextFloorTransition:
            break;

        case GameState.DisplayItemWindow:
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
        _floor.UpdateMinimap(_player.Loc, _enemies, _fieldItems);
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

    public bool RemoveFieldItem(FieldItem fieldItem) {
        for (int i = 0; i < _fieldItems.Count; i++) {
            if (_fieldItems[i].Loc == fieldItem.Loc) {
                Assert.IsTrue(System.Object.ReferenceEquals(_fieldItems[i], fieldItem));
                fieldItem.Destroy();
                _fieldItems.RemoveAt(i);
                return true;
            }
        }
        return false;
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

    public Loc RandomRoomLoc(Loc defaultLoc) {
        var rand = new System.Random();
        const int retryCount = 20;
        for (int i = 0; i < retryCount; i++) {
            var room = Utils.Choice(_floor.GetRooms());

            int r = rand.Next(room.Row, room.Row + room.Height);
            int c = rand.Next(room.Col, room.Col + room.Width);
            var loc = new Loc(r, c);
            if (!_enemies.Any(e => e.Loc == loc)) {
                return loc;
            }
        }
        return defaultLoc;
    }

    public void IncGold(int gold) {
        _gold += gold;

        var text = GameObject.Find("Canvas/Header/Text_G_Value").GetComponent<Text>();
        text.text = _gold.ToString();
    }

    // // loc から front に向かって最初にヒットする CharacterBase を返す
    // public CharacterBase FindHitTarget(Loc loc, Dir front) {
    //     int distance = 100; // 探索する距離
    //     for (int i = 0; i < distance; i++) {
    //         Loc next = loc.Forward(front);

    //         if (_floor.IsWall(next) || _floor.ExistsObstacle(next)) break;

    //         int p = _enemies.FindIndex(e => e.Loc == next);
    //         if (p != -1) return _enemies[p];
    //         loc = next;
    //     }
    //     return null;
    // }
}
