public abstract class Act {
    public CharacterBase Actor { get; private set; }

    public bool Finished { get; protected set; }
    protected bool AnimationFinished { get; set; }

    private bool _started;

    public abstract void RunEffect(MainSystem sys);

    public virtual void RunAnimation(MainSystem sys) {
        // アニメーションが完了したら、AnimationFinished を true にする必要がある
        // TODO:終了をどのように検知するか
        AnimationFinished = true;
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
            RunEffect(sys);
            Actor.ActCount--;
            Finished = true;
        }
    }
}
