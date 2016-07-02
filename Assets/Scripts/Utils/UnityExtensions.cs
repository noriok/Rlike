using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public static class UnityExtensions {

    public static IEnumerator Fade(this MaskableGraphic graphic, float fm, float to, float duration) {
        var color = graphic.color;
        yield return CAction.Lerp(duration, fm, to, alpha => {
            color.a = alpha;
            graphic.color = color;
        });
    }
}
