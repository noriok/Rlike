using UnityEngine;
using System;
using System.Collections;

public class MainSystem : MonoBehaviour {
    public GameObject obj1, obj2;

    void Start() {

    }

    void Update() {
    }

    void OnGUI() {
        Func<string, bool> button = (caption) => {
            return GUILayout.Button(caption, GUILayout.Width(110), GUILayout.Height(50));
        };

        if (button("Test 1")) {
            StartCoroutine(Move(obj1, 3, 0));
            StartCoroutine(Move(obj2, 2, 0));

        }
        else if (button("Test 2")) {

        }
    }

    IEnumerator Move(GameObject obj, int dx, int dy) {
        var src = obj.transform.position;

        float duration = 1.0f;
        float elapsed = 0;
        while (elapsed <= duration) {
            float x = Mathf.Lerp(src.x, src.x + dx, elapsed / duration);
            float y = Mathf.Lerp(src.y, src.y + dy, elapsed / duration);
            obj.transform.position = new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
