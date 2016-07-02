// using System;
using UnityEngine;

public static class EnemyFactory {
    public static Enemy Create(Loc loc) {
        var layer = LayerManager.GetLayer(LayerName.Enemy);

        string[] names = new[] { "hone_0", "obake_0" };
        var name = names[new System.Random().Next(names.Length)];

        var obj = Res.Create("Prefabs/Animations/" + name);
        // var gobj = (GameObject)GameObject.Instantiate(obj);
        obj.transform.SetParent(layer.transform);
        return new Enemy(loc, obj);
    }
}
