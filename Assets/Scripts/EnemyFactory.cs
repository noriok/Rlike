// using System;
using UnityEngine;

public static class EnemyFactory {
    public static Enemy CreateEnemy(Loc loc) {
        var layer = LayerManager.GetLayer(LayerName.Enemy);

        string[] names = new[] { "hone_0", "obake_0" };
        var name = names[new System.Random().Next(names.Length)];

        var obj = Resources.Load("Prefabs/Animations/" + name);
        var gobj = (GameObject)GameObject.Instantiate(obj);
        gobj.transform.SetParent(layer.transform);
        return new Enemy(loc, gobj);
    }
}
