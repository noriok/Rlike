using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FloorBanner {
	private GameObject _banner;

	public FloorBanner() {
		_banner = GameObject.Find("Canvas/FloorBanner");
		_banner.SetActive(false);
	}

	public IEnumerator FadeInAnimation(string floorName, int floorNumber) {
		_banner.SetActive(true);
		var image = GameObject.Find("Canvas/FloorBanner/Image").GetComponent<Image>();
		var text = GameObject.Find("Canvas/FloorBanner/Text").GetComponent<Text>();

		text.text = "テストダンジョン\n1F";
		var color = text.color;
		color.a = 0;
		text.color = color;

		// マスク、フェードイン
		yield return image.Fade(0, 1, 0.4f);

		yield return new WaitForSeconds(0.2f);

		// バナー、フェードイン
		yield return text.Fade(0, 1, 0.6f);
	}

	public IEnumerator FadeOutAnimation() {
		var image = GameObject.Find("Canvas/FloorBanner/Image").GetComponent<Image>();
		var text = GameObject.Find("Canvas/FloorBanner/Text").GetComponent<Text>();

		// マスク、フェードアウト
		yield return image.Fade(1, 0, 0.7f);

		// バナー、フェードアウト
		yield return text.Fade(1, 0, 0.2f);

		_banner.SetActive(false);
	}
}
