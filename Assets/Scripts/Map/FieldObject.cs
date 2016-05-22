using UnityEngine;
using System.Collections;

public abstract class FieldObject {
    public Loc Loc { get; private set; }
    public bool Visible {
        get { return _isVisible; }
        set {
            if (_isVisible != value) {
                _isVisible = value;
                _gobj.SetActive(value);
            }
        }
    }

    private GameObject _gobj;
    private bool _isVisible;

    public FieldObject(Loc loc, GameObject gobj) {
        Loc = loc;
        _gobj = gobj;
        _isVisible = true;
    }

    public virtual IEnumerator RunAnimation(CharacterBase sender, MainSystem sys) {
        yield break;
    }

    public virtual bool IsObstacle() {
        return true;
    }
}
