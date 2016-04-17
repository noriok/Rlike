using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static class EffectAnim {
    public static IEnumerator PopupWhiteDigits(int n, Vector3 pos, Action finishedCallback) {
        return PopupDigits(n, pos, finishedCallback, "Prefabs/Digits/digits_white_");
    }

    public static IEnumerator PopupGreenDigits(int n, Vector3 pos, Action finishedCallback) {
        return PopupDigits(n, pos, finishedCallback, "Prefabs/Digits/digits_green_");
    }

    private static IEnumerator PopupDigits(int n, Vector3 pos, Action finishedCallback, string pathPrefix) {
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

                    if (Mathf.Abs(v) < 0.016f) {
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

		finishedCallback();
    }

}
