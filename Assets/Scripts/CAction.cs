using UnityEngine;
using System;
using System.Collections;

public static class CAction {
	public static IEnumerator Walk(CharacterBase target, int drow, int dcol, Action<float, float> updateCallback) {
		var src = target.Position;
	    float duration = Config.WalkDuration;
        float elapsed = 0;
        float dx = dcol * Config.ChipSize;
        float dy = drow * Config.ChipSize;
        while (elapsed <= duration) {
            elapsed += Time.deltaTime;
            float x = Mathf.Lerp(src.x, src.x + dx, elapsed / duration);
            float y = Mathf.Lerp(src.y, src.y - dy, elapsed / duration);
            target.Position = new Vector3(x, y, 0);


			if (updateCallback != null) updateCallback(x, y);
            yield return null;
        }

	    // 位置ずれしないように最終位置にセット
        float x2 = src.x + dx;
        float y2 = src.y - dy;
        target.Position = new Vector3(x2, y2, 0);
		if (updateCallback != null) updateCallback(x2, y2);
	}

    public static IEnumerator Move(GameObject target, Loc fm, Loc to) {
        var src = fm.ToPosition();
        var dst = to.ToPosition();

        float speed = Config.ThrowItemSpeed;
        if (fm.Row != to.Row && fm.Col != to.Col) {
            speed *= 1.2f;
        }

        float elapsed = 0;
        float duration = Vector3.Distance(src, dst) / speed;
        while (elapsed <= duration) {
            elapsed += Time.deltaTime;

            float x = Mathf.Lerp(src.x, dst.x, elapsed / duration);
            float y = Mathf.Lerp(src.y, dst.y, elapsed / duration);
            target.transform.position = new Vector3(x, y, 0);
            yield return null;
        }
        target.transform.position = dst;
    }

    public static IEnumerator MovePlayer(Player player, Loc to) {
        Loc fm = player.Loc;
        var src = fm.ToPosition();
        var dst = to.ToPosition();

        float speed = Config.ThrowItemSpeed;
        if (fm.Row != to.Row && fm.Col != to.Col) {
            speed *= 1.2f;
        }

        float elapsed = 0;
        float duration = Vector3.Distance(src, dst) / speed;
        while (elapsed <= duration) {
            elapsed += Time.deltaTime;

            float x = Mathf.Lerp(src.x, dst.x, elapsed / duration);
            float y = Mathf.Lerp(src.y, dst.y, elapsed / duration);

            player.Position = new Vector3(x, y, 0);
            player.SyncCameraPosition();
            yield return null;
        }
        player.UpdateLoc(to);
    }

    public static IEnumerator Quake(GameObject layer, float duration) {
        float d = 0.015f;
        var offsets = new[,] {
            { -d, d }, { 0, 0 }, { d, -d }, { 0, 0 }, { d, d }, { 0, 0 }, { -d, -d },
        };

        var src = layer.transform.position;
        float elapsed = 0;
        while (elapsed < duration) {
            for (int i = 0; i < offsets.GetLength(0); i++) {
                if (elapsed >= duration) break;

                var pos = new Vector3(src.x + offsets[i, 0], src.y + offsets[i, 1], src.z);
                layer.transform.position = pos;
                yield return new WaitForSeconds(0.014f);
                elapsed += Time.deltaTime;

            }
        }
        layer.transform.position = src;
    }

    public static IEnumerator Fade(GameObject obj, float fm, float to, float duration) {
        var renderer = obj.GetComponent<SpriteRenderer>();
        var color = renderer.color;
        color.a = fm;
        renderer.color = color;

        float elapsed = 0f;
        while (elapsed <= duration) {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(fm, to, elapsed / duration);
            color.a = alpha;
            renderer.color = color;
            yield return null;
        }
    }
}
