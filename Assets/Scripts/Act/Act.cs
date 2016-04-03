public abstract class Act {
    public CharacterBase Actor { get; private set; }

    public int Priority { get { return GetPriority(); } }
    public bool Finished { get; protected set; }
    protected bool AnimationFinished { get; set; }

    private bool _started;

    public abstract void RunEffect(MainSystem sys);
    protected abstract int GetPriority();

    public virtual void RunAnimation(MainSystem sys) {
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
