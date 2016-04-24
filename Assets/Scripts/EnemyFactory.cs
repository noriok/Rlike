// using System;
using UnityEngine;

public static class EnemyFactory {
    public static Enemy CreateEnemy(int row, int col) {
        string[] names = new[] { "hone_0", "obake_0" };
        var name = names[new System.Random().Next(names.Length)];

        var obj = Resources.Load("Prefabs/Animations/" + name);
        var gobj = (GameObject)GameObject.Instantiate(obj, new Vector3(0, 1, 0), Quaternion.identity);
        return new Enemy(row, col, gobj);
    }
}
