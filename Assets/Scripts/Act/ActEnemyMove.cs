﻿using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class ActEnemyMove : Act {
    private int _drow;
    private int _dcol;

    private Vector3 _srcPos;
    private Vector3 _dstPos;
    private float _elapsed;
    private float _duration = Config.WalkDuration;
    private bool _isFirst = true;

    // nextLoc = 移動後の位置
    public ActEnemyMove(Enemy enemy, Loc nextLoc) : base(enemy) {
        Assert.IsTrue(enemy.Loc.IsNeighbor(nextLoc));

        _drow = nextLoc.Row - enemy.Row;
        _dcol = nextLoc.Col - enemy.Col;

        _srcPos = enemy.Loc.ToPosition();
        _dstPos = nextLoc.ToPosition();
        _elapsed = 0;
    }

    public override bool IsMoveAct() {
        return true;
    }

    public override bool IsManualUpdate() {
        return true;
    }

    protected override IEnumerator RunAnimation(MainSystem sys) {
        Actor.ChangeDir(Utils.ToDir(_drow, _dcol));
        yield return CAction.Walk(Actor, _drow, _dcol, null);
    }

    public override void Apply(MainSystem sys) {
        var nextLoc = Actor.Loc + new Loc(_drow, _dcol);
        DLog.D("{0} move {1} -> {2}", Actor, Actor.Loc, nextLoc);
        Actor.UpdateLoc(nextLoc);
    }

    public override void Update(MainSystem sys) {
        if (_isFirst) {
            Actor.ChangeDir(Utils.ToDir(_drow, _dcol));
            _isFirst = false;
        }

        float t = _elapsed / _duration;
        float x = Mathf.Lerp(_srcPos.x, _dstPos.x, t);
        float y = Mathf.Lerp(_srcPos.y, _dstPos.y, t);
        Actor.Position = new Vector3(x, y, 0);

        if (_elapsed >= _duration) {
            _animationFinished = true;

            // 位置ずれ防止
            Actor.Position = _dstPos;
        }
        _elapsed += Time.deltaTime;
    }
}
