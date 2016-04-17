using UnityEngine;

public class Enemy : CharacterBase {
    private const float HP_GAUGE_MAX_SCALE = 60.0f;
    private GameObject _barGreen;

    public Enemy(int row, int col, GameObject gobj) : base(row, col, gobj) {

        // HP バー
        var barRed = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/HpBar/bar-red"), Vector3.zero, Quaternion.identity);
        barRed.transform.SetParent(gobj.transform);
        barRed.transform.localPosition = new Vector3(0, 0.18f, 0);

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

    public override string ToString() {
        return string.Format("E:{0}", Loc);
    }
}
