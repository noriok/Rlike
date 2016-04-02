using UnityEngine;
using System.Collections;

public static class EnemyFactory {
    public static Enemy CreateEnemy() {
        var obj = Resources.Load("Prefabs/piece5");
        Debug.Log("gobj = " + obj);

        var gobj = (GameObject)GameObject.Instantiate(obj, new Vector3(0, 1, 0), Quaternion.identity);
        return new Enemy(0, 0, gobj);
    }
}
