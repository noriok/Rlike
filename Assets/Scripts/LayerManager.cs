using UnityEngine;
using UnityEngine.Assertions;
// using System.Collections;

public class LayerManager {

    private static string[] _layerNames = new[] {
        LayerName.Enemy,
        LayerName.Item,
        LayerName.FieldObject,
        LayerName.Trap,
        LayerName.Map,
        LayerName.Minimap,
    };

    public static void CreateAllLayer() {
        foreach (var name in _layerNames) {
            new GameObject(name);
        }
    }

    public static void CreateLayer(string layerName) {
        new GameObject(layerName);
    }

    public static void RemoveAllLayer() {
        foreach (var name in _layerNames) {
            GameObject.Destroy(GameObject.Find(name));
        }
    }

    public static void RemoveLayer(string layerName) {
        GameObject.Destroy(GameObject.Find(layerName));
    }

    public static GameObject GetLayer(string layerName) {
        var layer = GameObject.Find(layerName);
        Assert.IsTrue(layer != null);
        return layer;
    }
}
