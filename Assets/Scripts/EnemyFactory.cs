using UnityEngine;

public static class EnemyFactory {
    public static Enemy CreateEnemy(int row, int col) {
        // var obj = Resources.Load("Prefabs/Character/hone_1");
        var obj = Resources.Load("Prefabs/Animations/hone_3_W");

        var gobj = (GameObject)GameObject.Instantiate(obj, new Vector3(0, 1, 0), Quaternion.identity);
        return new Enemy(row, col, gobj);
    }
}
