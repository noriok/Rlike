using UnityEngine;

public static class EnemyFactory {
    public static Enemy CreateEnemy(int row, int col) {
        var obj = Resources.Load("Prefabs/Animations/hone_0");

        var gobj = (GameObject)GameObject.Instantiate(obj, new Vector3(0, 1, 0), Quaternion.identity);

        // HP バー
        var barRed = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/HpBar/bar-red"), Vector3.zero, Quaternion.identity);
        barRed.transform.SetParent(gobj.transform);
        barRed.transform.localPosition = new Vector3(0, 0.18f, 0);

        var barGreen = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/HpBar/bar-green"), Vector3.zero, Quaternion.identity);
        barGreen.transform.SetParent(gobj.transform);
        barGreen.transform.localPosition = new Vector3(-0.15f, 0.18f, 0);
        barGreen.transform.localScale = new Vector3(60f / 3, 1, 1);

        return new Enemy(row, col, gobj);
    }
}
