// using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AnimWrapper {
	public bool Finished { get; private set; }

	private IEnumerator Run(Func<IEnumerator> Fn) {
		yield return Fn();
		Finished = true;
	}

	public AnimWrapper(MainSystem sys, Func<IEnumerator> fn) {
		sys.StartCoroutine(Run(fn));
	}
}

public static class Anim {
	// 並列に実行。
	public static IEnumerator Par(MainSystem sys, params Func<IEnumerator>[] args) {
		var xs = new List<AnimWrapper>();
		foreach (var a in args) {
			xs.Add(new AnimWrapper(sys, a));
		}

	    while (true) {
			bool finished = true;
			foreach (var a in xs) {
				finished = finished && a.Finished;
			}
			if (finished) yield break;
			yield return null;
		}
	}
}
