using System.Collections;
using UnityEngine;

public class Enemy : CharacterBase {
    private const float HP_GAUGE_MAX_SCALE = 60.0f;
    private GameObject _barGreen;
    private GameObject _barYellow;

    public bool IsLockedOn { get; private set; }
    public Loc Target { get; private set; }

    public Enemy(Loc loc, GameObject gobj) : base(loc, gobj) {
        // HP バー
        var barRed = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/HpBar/bar-red"), Vector3.zero, Quaternion.identity);
        barRed.transform.SetParent(gobj.transform);
        barRed.transform.localPosition = new Vector3(0, 0.18f, 0);

        var barYellow = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/HpBar/bar-yellow"), Vector3.zero, Quaternion.identity);
        barYellow.transform.SetParent(gobj.transform);
        barYellow.transform.localPosition = new Vector3(-0.15f, 0.18f, 0);
        barYellow.transform.localScale = new Vector3(HP_GAUGE_MAX_SCALE, 1, 1);
        _barYellow = barYellow;

        var barGreen = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/HpBar/bar-green"), Vector3.zero, Quaternion.identity);
        barGreen.transform.SetParent(gobj.transform);
        barGreen.transform.localPosition = new Vector3(-0.15f, 0.18f, 0);
        barGreen.transform.localScale = new Vector3(HP_GAUGE_MAX_SCALE, 1, 1);
        _barGreen = barGreen;
    }

    public override void DamageHp(int delta) {
        base.DamageHp(delta);

        float scale = Hp * HP_GAUGE_MAX_SCALE / MaxHp;
        _barGreen.transform.localScale = new Vector3(scale, 1, 1);
    }

    public override IEnumerator HealAnim(int delta) {
        int hp = Utils.Clamp(Hp + delta, 0, MaxHp);
        float scale = hp * HP_GAUGE_MAX_SCALE / MaxHp;
        _barGreen.transform.localScale = new Vector3(scale, 1, 1);
        yield return null;
    }

    public override IEnumerator DamageAnim(int delta) {
        int fm = Hp;
        int to = Utils.Clamp(Hp - delta, 0, MaxHp);

        float greenScale = to * HP_GAUGE_MAX_SCALE / MaxHp;
        _barGreen.transform.localScale = new Vector3(greenScale, 1, 1);
        yield return new WaitForSeconds(0.43f);

        float duration = 0.3f;
        float elapsed = 0;
        while (elapsed <= duration) {
            elapsed += Time.deltaTime;

            // float p = Mathf.Lerp(fm, to, elapsed / duration);
            float p = UTween.Ease(EaseType.OutQuad, fm, to, elapsed / duration);
            float scale = p * HP_GAUGE_MAX_SCALE / MaxHp;
            _barYellow.transform.localScale = new Vector3(scale, 1, 1);
            yield return null;
        }
    }

    public override string ToString() {
        return string.Format("E:{0}", Loc);
    }

    public override void OnTurnStart() {
        ActCount = 1;
    }

    public override void OnTurnEnd() {
        base.OnTurnEnd();

        if (IsLockedOn) {
            if (Loc == Target) {
                CancelTarget();
            }
        }
    }

    public void LockOn(Loc target) {
        IsLockedOn = true;
        Target = target;
    }

    public void CancelTarget() {
        IsLockedOn = false;
    }

    // 行動できないなら true
    public bool IsBehavioralIncapacitation() {
        if (IsSleep()) return true;
        if (IsFreeze()) return true;
        return false;
    }

    private void AddStatusSymbol(string pathName) {

    }

    public override void OnStatusAdded(StatusType status) {
        if (status == StatusType.Invisible) {
            _gobj.SetActive(false);
        }
    }

    public override void OnStatusRemoved(StatusType status) {
        if (status == StatusType.Invisible) {
            _gobj.SetActive(true);
        }
    }
}
