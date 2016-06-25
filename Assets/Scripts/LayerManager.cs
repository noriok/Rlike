using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

public class LayerManager {
    private static Dictionary<string, GameObject> _layerDictionary = new Dictionary<string, GameObject>();

    private static string[] _layerNames = new[] {
        LayerName.Enemy,
        LayerName.Item,
        LayerName.FieldObject,
        LayerName.Trap,
        LayerName.Map,
        LayerName.Minimap,
        LayerName.SpotlightPassage,
        LayerName.SpotlightRoom,
    };

    public static void CreateAllLayer() {
        foreach (var name in _layerNames) {
            CreateLayer(name);
        }
    }

    public static void CreateLayer(string layerName) {
        Assert.IsTrue(!_layerDictionary.ContainsKey(layerName));
        var layer = new GameObject(layerName);
        _layerDictionary[layerName] = layer;
    }

    public static void RemoveAllLayer() {
        foreach (var layer in _layerDictionary.Values) {
            Assert.IsTrue(layer != null);
            GameObject.Destroy(layer);
        }
        _layerDictionary.Clear();
    }

    public static void RemoveLayer(string layerName) {
        if (_layerDictionary.ContainsKey(layerName)) {
            var layer = _layerDictionary[layerName];
            Assert.IsTrue(layer != null);
            GameObject.Destroy(layer);
            _layerDictionary.Remove(layerName);
        }
    }

    public static GameObject GetLayer(string layerName) {

        var layer = _layerDictionary[layerName];
        Assert.IsTrue(layer != null);
        return layer;
    }
}
