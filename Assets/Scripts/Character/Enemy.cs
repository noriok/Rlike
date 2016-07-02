using System.Collections;
using UnityEngine;

// BUG:TODO:通路で睡眠草を投げてモンスターのアニメーションを停止させてから
//     アクティブを切り替えると、モンスターの向きがおかしくなる。
//     アクティブを true にしたときにアニメーションの初期化が走っている？
public class Enemy : CharacterBase {
    private const float HP_GAUGE_MAX_SCALE = 60.0f;
    private GameObject _barGreen;
    private GameObject _barYellow;

    public bool IsLockedOn { get; private set; }
    public Loc TargetLoc { get; private set; }
    public Loc NextLoc { get; private set; } // 次のターンにおけるモンスターの座標

    public bool CanLongDistanceAttack { get; set; }

    public Enemy(Loc loc, GameObject gobj) : base(loc, gobj) {
        // HP バー
        var barRed = Res.Create("Prefabs/HpBar/bar-red");
        barRed.transform.SetParent(gobj.transform);
        barRed.transform.localPosition = new Vector3(0, 0.18f, 0);

        var barYellow = Res.Create("Prefabs/HpBar/bar-yellow");
        barYellow.transform.SetParent(gobj.transform);
        barYellow.transform.localPosition = new Vector3(-0.15f, 0.18f, 0);
        barYellow.transform.localScale = new Vector3(HP_GAUGE_MAX_SCALE, 1, 1);
        _barYellow = barYellow;

        var barGreen = Res.Create("Prefabs/HpBar/bar-green");
        barGreen.transform.SetParent(gobj.transform);
        barGreen.transform.localPosition = new Vector3(-0.15f, 0.18f, 0);
        barGreen.transform.localScale = new Vector3(HP_GAUGE_MAX_SCALE, 1, 1);
        _barGreen = barGreen;

        NextLoc = loc;
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
        yield return CAction.Run(duration, elapsed => {
            float p = UTween.Ease(EaseType.OutQuad, fm, to, elapsed / duration);
            float scale = p * HP_GAUGE_MAX_SCALE / MaxHp;
            _barYellow.transform.localScale = new Vector3(scale, 1, 1);
        });
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
            if (Loc == TargetLoc) {
                CancelTarget();
            }
        }

        // 次のターンでの移動座標を初期化する
        NextLoc = Loc;
    }

    public void UpdateNextLoc(Loc nextLoc) {
        NextLoc = nextLoc;
    }

    public void LockOn(Loc targetLoc) {
        // Debug.LogFormat("--- ロックオン: {0}", targetLoc);
        IsLockedOn = true;
        TargetLoc = targetLoc;
    }

    public void CancelTarget() {
        // Debug.LogFormat("--- ロックオン解除: {0} Dir:{1}", TargetLoc, Dir);
        IsLockedOn = false;
    }

    // TODO:rename
    // 行動できないなら true
    public bool IsBehavioralIncapacitation() {
        if (IsSleep()) return true;
        if (IsFreeze()) return true;
        return false;
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
