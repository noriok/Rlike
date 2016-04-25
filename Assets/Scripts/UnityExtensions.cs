using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public static class UnityExtensions {

	public static IEnumerator Fade(this Image image, float fm, float to, float duration) {
		var color = image.color;
		float elapsed = 0;
		while (elapsed <= duration) {
			elapsed += Time.deltaTime;
			float alpha = Mathf.Lerp(fm, to, elapsed / duration);
			color.a = alpha;
			image.color = color;
			yield return null;
		}
		color.a = to;
		image.color = color;
	}

	public static IEnumerator Fade(this Text text, float fm, float to, float duration) {
		var color = text.color;
		float elapsed = 0;
		while (elapsed <= duration) {
			elapsed += Time.deltaTime;
			float alpha = Mathf.Lerp(fm, to, elapsed / duration);
			color.a = alpha;
			text.color = color;
			yield return null;
		}
		color.a = to;
		text.color = color;
	}
}
