using UnityEngine;

public static class EnemyFactory {
    public static Enemy CreateEnemy(int row, int col) {
        var obj = Resources.Load("Prefabs/Character/hone_1");

        var gobj = (GameObject)GameObject.Instantiate(obj, new Vector3(0, 1, 0), Quaternion.identity);
        return new Enemy(row, col, gobj);
    }
}
