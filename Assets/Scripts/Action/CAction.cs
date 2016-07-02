using UnityEngine;
using System;
using System.Collections;

public static class CAction {
    public static IEnumerator Run(float duration, Action<float> action) {
        float elapsed = 0;
        while (elapsed <= duration) {
            action(elapsed);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public static IEnumerator Move(GameObject target, Loc fm, Loc to) {
        var src = fm.ToPosition();
        var dst = to.ToPosition();

        float speed = fm.IsDiagonal(to) ? Config.DiagonalMoveSpeed : Config.MoveSpeed;
        float duration = Vector3.Distance(src, dst) / speed;
        yield return Lerp(duration, src, dst, pos => {
            target.transform.position = pos;
        });
        target.transform.position = dst;
    }

    // カメラ追従
    public static IEnumerator MovePlayer(Player player, Loc to) {
        Loc fm = player.Loc;
        var src = fm.ToPosition();
        var dst = to.ToPosition();
        float speed = fm.IsDiagonal(to) ? Config.DiagonalMoveSpeed : Config.MoveSpeed;
        float duration = Vector3.Distance(src, dst) / speed;
        yield return Lerp(duration, src, dst, pos => {
            player.Position = pos;
            player.SyncCameraPosition();
        });
        player.UpdateLoc(to);
    }

    public static IEnumerator MoveEnemy(Enemy enemy, Loc to) {
        Loc fm = enemy.Loc;
        var src = fm.ToPosition();
        var dst = to.ToPosition();

        float speed = fm.IsDiagonal(to) ? Config.DiagonalMoveSpeed : Config.MoveSpeed;
        float duration = Vector3.Distance(src, dst) / speed;
        yield return Lerp(duration, src, dst, pos => {
            enemy.Position = pos;
        });
        enemy.UpdateLoc(to);
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
        yield return Run(duration, elapsed => {
            float alpha = Mathf.Lerp(fm, to, elapsed / duration);
            color.a = alpha;
            renderer.color = color;
        });
    }

    public static IEnumerator Lerp(float duration, Vector3 src, Vector3 dst, Action<Vector3> action) {
        Vector3 v = Vector3.zero;
        float elapsed = 0;
        while (elapsed <= duration) {
            float r = elapsed / duration;
            v.x = Mathf.Lerp(src.x, dst.x, r);
            v.y = Mathf.Lerp(src.y, dst.y, r);
            action(v);
            elapsed += Time.deltaTime;
            yield return null;
        }
        action(dst);
    }

    public static IEnumerator Lerp(float duration, float fm, float to, Action<float> action) {
        float elapsed = 0;
        while (elapsed <= duration) {
            action(Mathf.Lerp(fm, to, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }
        action(to);
    }
}
