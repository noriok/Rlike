using UnityEngine;
using System.Collections;

public class ActPlayerMove : Act {
    private int _drow;
    private int _dcol;

    private Camera _camera;
    private float _cameraZ;
    private GameObject _minimapLayer;
    private float _elapsed;
    private Vector3 _srcPos;
    private Vector3 _dstPos;
    private float _duration = Config.WalkDuration;

    private bool _isFirst = true;

    public ActPlayerMove(Player player, int drow, int dcol) : base(player) {
        _drow = drow;
        _dcol = dcol;

        _camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        _cameraZ = _camera.transform.position.z;
        _minimapLayer = GameObject.Find(LayerName.Minimap);

        _elapsed = 0;
        _srcPos = player.Loc.ToPosition();
        _dstPos = (player.Loc + new Loc(drow, dcol)).ToPosition();
    }

    public override bool IsMoveAct() {
        return true;
    }

    protected override IEnumerator RunAnimation(MainSystem sys) {
        Actor.ChangeDir(Utils.ToDir(_drow, _dcol));

        // TODO: 一歩一歩が連続していないので矢印が点滅してしまう
        //Actor.ShowDirection(Utils.ToDir(_drow, _dcol));

        var camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        float cameraZ = camera.transform.position.z;
        var minimap = GameObject.Find("Minimap Layer");
        yield return CAction.Walk(Actor, _drow, _dcol, (x, y) => {
            var pos = camera.transform.position;

            // カメラの位置も合わせる
            camera.transform.position = new Vector3(x, y + Config.CameraOffsetY, cameraZ);

            // ミニマップの位置
            float dx = camera.transform.position.x - pos.x;
            float dy = camera.transform.position.y - pos.y;
            var t = minimap.transform.position;
            minimap.transform.position = new Vector3(t.x + dx, t.y + dy, t.z);
        });

        // Actor.HideDirection();
    }

    public override void Apply(MainSystem sys) {
        var nextLoc = Actor.Loc + new Loc(_drow, _dcol);
        DLog.D("{0} move {1} -> {2}", Actor, Actor.Loc, nextLoc);
        Actor.UpdateLoc(nextLoc);
    }

    public override bool IsManualUpdate() {
        return true;
    }

    public override void Update(MainSystem sys) {
        if (_isFirst) {
            Actor.ChangeDir(Utils.ToDir(_drow, _dcol));
            _isFirst = false;
        }

        float x = Mathf.Lerp(_srcPos.x, _dstPos.x, _elapsed / _duration);
        float y = Mathf.Lerp(_srcPos.y, _dstPos.y, _elapsed / _duration);
        Actor.Position = new Vector3(x, y, 0);

        Vector3 cameraPos = _camera.transform.position;
        // カメラの位置をプレイヤーの位置に合わせる
        float cameraX = x;
        float cameraY = y + Config.CameraOffsetY;
        _camera.transform.position = new Vector3(cameraX, cameraY, _cameraZ);


        float dx = _camera.transform.position.x - cameraPos.x;
        float dy = _camera.transform.position.y - cameraPos.y;
        var t = _minimapLayer.transform.position;
        _minimapLayer.transform.position = new Vector3(t.x + dx, t.y + dy, t.z);

        if (_elapsed >= _duration) {
            _animationFinished = true;

            // 位置ずれ防止
            Actor.Position = _dstPos;
            _camera.transform.position = new Vector3(_dstPos.x, _dstPos.y + Config.CameraOffsetY, _cameraZ);
            // TODO:ミニマップの位置


            Debug.Log("walk end " + Time.time);
        }
        _elapsed += Time.deltaTime;
    }
}
