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
        LayerName.SpotRoom,
        LayerName.SpotPassage,
    };

    public static void CreateAllLayer() {
        foreach (var name in _layerNames) {
            CreateLayer(name);
        }
    }

    public static void CreateLayer(string layerName) {
        // Debug.LogFormat("--> Create layer:{0}", layerName);
        new GameObject(layerName);
    }

    public static void RemoveAllLayer() {
        foreach (var name in _layerNames) {
            RemoveLayer(name);
        }
    }

    public static void RemoveLayer(string layerName) {
        var layer = GameObject.Find(layerName);
        Assert.IsTrue(layer != null);
        // Debug.Log("--> destroy layer = " + layer);
        GameObject.Destroy(layer);
    }

    public static GameObject GetLayer(string layerName) {
        var layer = GameObject.Find(layerName);
        Assert.IsTrue(layer != null);
        return layer;
    }
}
