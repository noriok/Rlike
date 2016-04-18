using UnityEngine;
// using System;
using System.Collections;
using System.Collections.Generic;

public static class EffectAnim {
    public static IEnumerator PopupWhiteDigits(CharacterBase target, int n) {
        var pos = target.Position;
        pos.y -= 0.09f;
        return PopupDigits(pos, n, "Prefabs/Digits/digits_white_");
    }

    public static IEnumerator PopupGreenDigits(CharacterBase target, int n) {
        var pos = target.Position;
        pos.y -= 0.09f;
        return PopupDigits(pos, n, "Prefabs/Digits/digits_green_");
    }

    private static IEnumerator PopupDigits(Vector3 pos, int n, string pathPrefix) {
        float fontWidth = 0.14f;

        var digits = new List<GameObject>();
        var ds = Utils.Digits(n);
        float x = pos.x - fontWidth * ds.Length / 2.0f + fontWidth / 2;
        foreach (var d in ds) {
            var obj = Resources.Load(pathPrefix + d);
            var gobj = (GameObject)GameObject.Instantiate(obj, new Vector3(x, pos.y, pos.z), Quaternion.identity);
            digits.Add(gobj);
            x += fontWidth;
        }

        float v = -0.059f; // velocity
        float g = 0.008f; // gravity
		g *= 2;
        float elapsed = 0;

        int frame = 0;
        float y = pos.y;
        while (true) {
            int f = (int)(elapsed / 0.033f);
            if (frame < f) {
                frame++;
                y -= v;
                v += g;
                if (y <= pos.y) {
                    v *= -0.45f;
                    y = pos.y;

                    if (Mathf.Abs(v) < 0.016f + 0.01) {
                        v = 0;
                        foreach (var digit in digits) {
                            var p = digit.transform.position;
                            p.y = y;
                            digit.transform.position = p;
                        }
                        yield return new WaitForSeconds(0.4f);
                        foreach (var digit in digits) {
                            GameObject.Destroy(digit);
                        }
                        break;
                    }
                }

                foreach (var digit in digits) {
                    var p = digit.transform.position;
                    p.y = y;
                    digit.transform.position = p;
                }
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public static IEnumerator Dead(Vector3 pos) {
	    var obj = Resources.Load("Prefabs/Animations/dead");
		var gobj = (GameObject)GameObject.Instantiate(obj, pos, Quaternion.identity);
		while (gobj != null) {
			yield return null;
		}
    }

    public static IEnumerator Heal(CharacterBase target) {
		var obj = Resources.Load("Prefabs/Animations/heal");
		var gobj = (GameObject)GameObject.Instantiate(obj, target.Position, Quaternion.identity);
		var pos = gobj.transform.position;
		pos.y += 0.15f;
		gobj.transform.position = pos;
        while (gobj != null) {
            yield return null;
        }
    }

    public static IEnumerator Aura(CharacterBase target) {
		var obj = Resources.Load("Prefabs/Animations/aura");
		var gobj = (GameObject)GameObject.Instantiate(obj, target.Position, Quaternion.identity);
		while (gobj != null) {
			yield return null;
		}
    }
}
