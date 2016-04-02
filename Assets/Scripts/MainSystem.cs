using UnityEngine;
using System;
using System.Collections;

/*

 j : 南  に移動 (移動先に敵がいる場合は、敵への攻撃)
 k : 北  に移動
 l : 東  に移動
 h : 西  に移動
 b : 南西に移動
 n : 南東に移動
 y : 北西に移動
 u : 北東に移動
 i : アイテムウィンドウを開く
 . : 何もせずにターン終了
 ; : 階段を下りる
 シフトキー + 方向キー : その方向へ矢を放つ(矢が必要)
 a : HP 表示メータのオン/オフ (デフォルトでオン)

※「方向キー」とは、上の 8 つのキー(j,k,l,h,b,n,y,u)のことです。

** アイテムウィンドウを開いて「いる」とき:

 h : アイテムを使用(または装備)
 i : アイテムウィンドウを閉じる
 j : カーソルを下に移動
 k : カーソルを上に移動
 o : アイテムをソート
 ; : アイテムを置く
 シフトキー + 方向キー : アイテムをその方向へ投げる

*/

public class MainSystem : MonoBehaviour {
    public GameObject obj1, obj2;

    private ActEnemyWalk _act;

    void Start() {
        var e = EnemyFactory.CreateEnemy();
        Debug.Log("e = " + e);

        var act = new ActEnemyWalk(e);
        _act = act;
    }

    void Update() {
    }

    void OnGUI() {
        Func<string, bool> button = (caption) => {
            return GUILayout.Button(caption, GUILayout.Width(110), GUILayout.Height(50));
        };

        if (button("Test 1")) {
            StartCoroutine(Move(obj1, 3, 0));
            StartCoroutine(Move(obj2, 2, -1));
        }
        else if (button("Test 2")) {
            StartCoroutine(_act.Exec());
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
