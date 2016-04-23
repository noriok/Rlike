using UnityEngine;
using System.Collections;

public abstract class Act {
    public CharacterBase Actor { get; private set; }
    public bool Finished { get; private set; }

    private bool _started;
    private bool _animationFinished;

    private IEnumerator StartAnimation(MainSystem sys) {
        yield return RunAnimation(sys);
        _animationFinished = true;
    }

    protected virtual IEnumerator RunAnimation(MainSystem sys) {
        yield break;
    }

    public virtual void Apply(MainSystem sys) {
    }

    // このタスクがキャラクターを移動させるタスクかどうか
    public virtual bool IsMoveAct() {
        return false;
    }

    public virtual bool IsTrapAct() {
        return false;
    }

    public Act(CharacterBase actor) {
        Actor = actor;
        _started = false;
        Finished = false;
    }

    public void UpdateAct(MainSystem sys) {
        if (Finished) return;

        if (!_started) {
            _started = true;
            Debug.LogFormat("run anim {0}", Actor);
            sys.StartCoroutine(StartAnimation(sys));
            return;
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
