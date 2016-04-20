using UnityEngine;
using System;
using System.Collections;

public static class CAction {
	public static IEnumerator Walk(CharacterBase target, int drow, int dcol, Action<float, float> updateCallback) {
		var src = target.Position;
	    float duration = 0.4f;
        float elapsed = 0;
        float dx = dcol * Config.ChipSize;
        float dy = drow * Config.ChipSize;
        while (elapsed <= duration) {
            float x = Mathf.Lerp(src.x, src.x + dx, elapsed / duration);
            float y = Mathf.Lerp(src.y, src.y - dy, elapsed / duration);
            target.Position = new Vector3(x, y, 0);
            elapsed += Time.deltaTime;

			if (updateCallback != null) updateCallback(x, y);
            yield return null;
        }

	    // 位置ずれしないように最終位置にセット
        float x2 = src.x + dx;
        float y2 = src.y - dy;
        target.Position = new Vector3(x2, y2, 0);
		if (updateCallback != null) updateCallback(x2, y2);
	}

}
