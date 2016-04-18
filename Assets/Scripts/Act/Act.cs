public abstract class Act {
    public CharacterBase Actor { get; private set; }

    public bool Finished { get; private set; }
    protected bool AnimationFinished { get; set; }

    private bool _started;

    public abstract void RunEffect(MainSystem sys);

    public virtual void RunAnimation(MainSystem sys) {
        // アニメーションが完了したら、AnimationFinished を true にする
        AnimationFinished = true;
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
    }

    public void UpdateAct(MainSystem sys) {
        if (Finished) return;

        if (!_started) {
            _started = true;
            RunAnimation(sys);
            return;
        }

        if (AnimationFinished) {
            // TODO:RunEffectで効果を実行。コールはサブクラスで行う
            RunEffect(sys);
            Actor.ActCount--;
            Finished = true;
        }
    }
}
