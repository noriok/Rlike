using UnityEngine;
using System.Collections;

public abstract class Act {
    public CharacterBase Actor { get; private set; }
    public bool Finished { get; private set; }

    private bool _started;
    protected bool _animationFinished;

    private IEnumerator StartAnimation(MainSystem sys) {
        yield return RunAnimation(sys);
        _animationFinished = true;
    }

    protected virtual IEnumerator RunAnimation(MainSystem sys) {
        yield break;
    }

    public virtual void Apply(MainSystem sys) {
    }

    // 他の Act の結果により、無効となった Act かどうか
    // たとえばワープで行動決定時とは異なる位置に移動した場合など。
    public virtual bool IsInvalid() {
        return false;
    }

    // このタスクがキャラクターを移動させるタスクかどうか
    public virtual bool IsMoveAct() {
        return false;
    }

    public virtual bool IsTrapAct() {
        return false;
    }

    public virtual bool IsManualUpdate() {
        return false;
    }

    public Act(CharacterBase actor) {
        Actor = actor;
        _started = false;
        Finished = false;
    }

    public virtual void Update(MainSystem sy) {
    }

    public void UpdateAct(MainSystem sys) {
        if (Finished) return;

        if (!_started) {
            _started = true;
            if (!IsManualUpdate()) {
                sys.StartCoroutine(StartAnimation(sys));
            }
        }

        if (IsManualUpdate()) {
            Update(sys);
        }

        //if (AnimationFinished) {
        if (_animationFinished) {
            // TODO:RunEffectで効果を実行。コールはサブクラスで行う
            Apply(sys);
            Actor.ActCount--;
            Finished = true;
        }
    }
}
