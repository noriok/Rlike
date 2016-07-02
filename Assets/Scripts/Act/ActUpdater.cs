// using UnityEngine;
// using System.Collections;
using System.Collections.Generic;

public class ActUpdater {

    private List<Act> _acts = new List<Act>();

    public bool IsEmpty() {
        return _acts.Count == 0;
    }

    public void Add(Act act) {
        _acts.Add(act);
    }

    public void AddRange(IEnumerable<Act> acts) {
        _acts.AddRange(acts);
    }

    public bool Update(MainSystem sys, Player player, List<Enemy> enemies, Floor floor) {
       // HP がゼロの敵は削除する。TODO:HP がゼロに鳴るタイミングは固定なので Update でチェックする必要はない。
        bool updated = true;
        while (updated) {
            updated = false;
            for (int i = 0; i < enemies.Count; i++) {
                if (enemies[i].Hp <= 0) {
                    enemies.RemoveAt(i);
                    updated = true;
                    break;
                }
            }
        }

        // 移動するキャラのタスクを先に実行する
        bool moveFinished = true;
        foreach (var act in _acts) {
            Assert.IsTrue(act.Actor.Hp > 0);
            // if (act.Actor.Hp <= 0) continue;
            if (act.IsMoveAct() && !act.Finished) {
                act.UpdateAct(sys);
                moveFinished = moveFinished && act.Finished;
            }
        }

        if (!moveFinished) return false; // 移動タスクが完了するまで待機

        bool trapFinished = true;
        foreach (var act in _acts) {
            if (act.IsTrapAct() && !act.Finished) {
                act.UpdateAct(sys);
                trapFinished = trapFinished && act.Finished;
            }
        }

        if (!trapFinished) return false; // トラップタスクが完了するまで待機

        // 移動タスク、トラップタスクが完了したら、それ以外の行動を一つずつ実行していく
        // TODO: act.IsInvalid() で無効になったキャラがいるなら、
        //       ただちに Detect で行動順を決めないと、攻撃の後に移動処理がくることになるはず。
        bool actFinished = true;
        foreach (var act in _acts) {
            if (act.Actor.Hp <= 0 || act.IsInvalid()) continue;
            if (act.Finished || act.IsMoveAct()) continue;

            act.UpdateAct(sys);
            actFinished = false;
            break; // 行動処理は 1 体ずつ行う
        }

        // 予約されている全てのタスクが終了した
        if (actFinished) {
            var acts = EnemyStrategy.Detect(enemies, player, player.Loc, floor);
            if (acts.Count > 0) { // 行動していないキャラがいる
                _acts.AddRange(acts);
                actFinished = false;
            }
        }
        return actFinished;
    }

    public void Clear() {
        _acts.Clear();
    }

}
