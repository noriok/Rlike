using System;

public enum EaseType {
    InQuad,
    OutQuad,
    InOutQuad,
    InCubic,
    OutCubic,
    InOutCubic,
    InQuart,
    OutQuart,
    InOutQuart,
    InQuint,
    OutQuint,
    InOutQuint,
    InSine,
    OutSine,
    InOutSine,
    InExpo,
    OutExpo,
    InOutExpo,
    InCirc,
    OutCirc,
    InOutCirc,
    InElastic,
    OutElastic,
    InOutElastic,
    InBack,
    OutBack,
    InOutBack,
    InBounce,
    OutBounce,
    InOutBounce,
}

public static class UTween {
    private static Func<float, float, float, float, float> GetEaseFn(EaseType easeType) {
        switch (easeType) {
        case EaseType.InQuad: return EaseInQuad;
        case EaseType.OutQuad: return EaseOutQuad;
        case EaseType.InOutQuad: return EaseInOutQuad;
        case EaseType.InCubic: return EaseInCubic;
        case EaseType.OutCubic: return EaseOutCubic;
        case EaseType.InOutCubic: return EaseInOutCubic;
        case EaseType.InQuart: return EaseInQuart;
        case EaseType.OutQuart: return EaseOutQuart;
        case EaseType.InOutQuart: return EaseInOutQuart;
        case EaseType.InQuint: return EaseInQuint;
        case EaseType.OutQuint: return EaseOutQuint;
        case EaseType.InOutQuint: return EaseInOutQuint;
        case EaseType.InSine: return EaseInSine;
        case EaseType.OutSine: return EaseOutSine;
        case EaseType.InOutSine: return EaseInOutSine;
        case EaseType.InExpo: return EaseInExpo;
        case EaseType.OutExpo: return EaseOutExpo;
        case EaseType.InOutExpo: return EaseInOutExpo;
        case EaseType.InCirc: return EaseInCirc;
        case EaseType.OutCirc: return EaseOutCirc;
        case EaseType.InOutCirc: return EaseInOutCirc;
        case EaseType.InElastic: return EaseInElastic;
        case EaseType.OutElastic: return EaseOutElastic;
        case EaseType.InOutElastic: return EaseInOutElastic;
        case EaseType.InBack: return EaseInBack;
        case EaseType.OutBack: return EaseOutBack;
        case EaseType.InOutBack: return EaseInOutBack;
        case EaseType.InBounce: return EaseInBounce;
        case EaseType.OutBounce: return EaseOutBounce;
        case EaseType.InOutBounce: return EaseInOutBounce;
        }
        throw new ArgumentException();
    }

    // t: 0 - 1.0
    public static float Ease(EaseType easeType, float beginVal, float endVal, float t) {
		if (t < 0) t = 0;
		else if (t > 1) t = 1;
        var easeFn = GetEaseFn(easeType);
        float duration = 1f;
        return easeFn(t, beginVal, endVal - beginVal, duration);
    }

	// t: current time, b: begInnIng value, c: change In value, d: duration

	// def: 'easeOutQuad',
	// swing: function (t, b, c, d) {
	// 	//alert(jQuery.easing.default);
	// 	return jQuery.easing[jQuery.easing.def](t, b, c, d);
	// },
	public static float EaseInQuad(float t, float b, float c, float d) {
        float u = t/d;
		return c * u * u + b;
	}

	public static float EaseOutQuad(float t, float b, float c, float d) {
        float u = t/d;
		return -c * u * (u-2) + b;
	}

	public static float EaseInOutQuad(float t, float b, float c, float d) {
        float u = t/(d/2);
		if (u < 1) return c/2 * u * u + b;
		return -c/2 * ((--u)*(t-2) - 1) + b;
	}

	public static float EaseInCubic(float t, float b, float c, float d) {
        float u = t/d;
		return c*u*u*u + b;
	}

	public static float EaseOutCubic(float t, float b, float c, float d) {
        float u = t/d-1;
		return c*(u*u*u + 1) + b;
	}

	public static float EaseInOutCubic(float t, float b, float c, float d) {
        float u = t/(d/2);
		if (u < 1f) return c/2*u*u*u + b;
        u -= 2;
		return c/2*(u*u*u + 2) + b;
	}

	public static float EaseInQuart(float t, float b, float c, float d) {
        float u = t/d;
		return c*u*u*u*u + b;
	}

	public static float EaseOutQuart(float t, float b, float c, float d) {
        float u = t/d-1;
		return -c * (u*u*u*u - 1) + b;
	}

	public static float EaseInOutQuart(float t, float b, float c, float d) {
        float u = t/(d/2);
		if (u < 1) return c/2*u*u*u*u + b;
        u -= 2;
		return -c/2 * (u*u*u*u - 2) + b;
	}

	public static float EaseInQuint(float t, float b, float c, float d) {
        float u = t/d;
		return c*u*u*u*u*u + b;
	}

	public static float EaseOutQuint(float t, float b, float c, float d) {
        float u = t/d-1;
		return c*(u*u*u*u*u + 1) + b;
	}

	public static float EaseInOutQuint(float t, float b, float c, float d) {
        float u = t/(d/2);
		if (u < 1) return c/2*u*u*u*u*u + b;
        u -= 2;
		return c/2*(u*u*u*u*u + 2) + b;
	}

	public static float EaseInSine(float t, float b, float c, float d) {
		return -c * (float)Math.Cos(t/d * (Math.PI/2)) + c + b;
	}

	public static float EaseOutSine(float t, float b, float c, float d) {
		return c * (float)Math.Sin(t/d * (Math.PI/2)) + b;
	}

	public static float EaseInOutSine(float t, float b, float c, float d) {
		return -c/2 * (float)(Math.Cos(Math.PI*t/d) - 1) + b;
	}

	public static float EaseInExpo(float t, float b, float c, float d) {
		return (t==0) ? b : c * (float)Math.Pow(2, 10 * (t/d - 1)) + b;
	}

	public static float EaseOutExpo(float t, float b, float c, float d) {
		return (t==d) ? b+c : c * (float)(-Math.Pow(2, -10 * t/d) + 1) + b;
	}

	public static float EaseInOutExpo(float t, float b, float c, float d) {
		if (t==0) return b;
		if (t==d) return b+c;

        float u = t/(d/2);
		if (u < 1) return c/2 * (float)Math.Pow(2, 10 * (u - 1)) + b;
		return c/2 * (float)(-Math.Pow(2, -10 * --u) + 2) + b;
	}

	public static float EaseInCirc(float t, float b, float c, float d) {
        float u = t/d;
		return -c * (float)(Math.Sqrt(1 - u*u) - 1) + b;
	}

	public static float EaseOutCirc(float t, float b, float c, float d) {
        float u = t/d-1;
		return c * (float)Math.Sqrt(1 - u*u) + b;
	}

	public static float EaseInOutCirc(float t, float b, float c, float d) {
        float u = t/(d/2);
		if (u < 1) return -c/2 * (float)(Math.Sqrt(1 - u*u) - 1) + b;
        u -= 2;
		return c/2 * (float)(Math.Sqrt(1 - u*u) + 1) + b;
	}

	public static float EaseInElastic(float t, float b, float c, float d) {
		float s=1.70158f;float p=0;float a=c;
		if (t==0) return b;
        float u = t/d;
        if (u==1) return b+c;  if (p == 0) p=d*.3f;
		if (a < Math.Abs(c)) { a=c; s=p/4; }
		else s = (float)(p/(2*Math.PI) * Math.Asin(c/a));
        u -= 1;
		return -(float)(a*Math.Pow(2,10*u) * Math.Sin((u*d-s)*(2*Math.PI)/p)) + b;
	}

	public static float EaseOutElastic(float t, float b, float c, float d) {
		float s=1.70158f; float p=0; float a=c;
		if (t==0) return b;

        float u = t/d;
        if (u==1) return b+c;  if (p==0) p=d*.3f;
		if (a < Math.Abs(c)) { a=c; s=p/4; }
		else s = (float)(p/(2*Math.PI) * Math.Asin(c/a));
		return (float)(a*Math.Pow(2,-10*u) * Math.Sin( (u*d-s)*(2*Math.PI)/p ) + c + b);
	}

	public static float EaseInOutElastic(float t, float b, float c, float d) {
		float s=1.70158f; float p=0; float a=c;
		if (t==0) return b;
        float u = t/(d/2);
        if (u==2) return b+c;  if (p==0) p=d*(.3f*1.5f);
		if (a < Math.Abs(c)) { a=c; s=p/4; }
		else s = (float)(p/(2*Math.PI) * Math.Asin(c/a));

		if (u < 1) {
            u -= 1;
            return (float)(-.5f*(a*Math.Pow(2,10*u) * Math.Sin((u*d-s)*(2*Math.PI)/p)) + b);
        }
        u -= 1;
		return (float)(a*Math.Pow(2,-10*u) * Math.Sin((u*d-s)*(2*Math.PI)/p)*.5 + c + b);
	}

	public static float EaseInBack(float t, float b, float c, float d) {
		float s = 1.70158f;
        float u = t/d;
		return c*u*u*((s+1)*u - s) + b;
	}

	public static float EaseOutBack(float t, float b, float c, float d) {
		float s = 1.70158f;
        float u = t/d-1;
		return c*(u*u*((s+1)*u + s) + 1) + b;
	}

	public static float EaseInOutBack(float t, float b, float c, float d) {
		float s = 1.70158f;
        float u = t/(d/2);
		if (u < 1) {
            s *= 1.525f;
            return c/2*(u*u*((s+1)*u - s)) + b;
        }
        u -= 2;
        s *= 1.525f;
		return c/2*(u*u*((s+1)*u + s) + 2) + b;
	}

	public static float EaseInBounce(float t, float b, float c, float d) {
		return c - EaseOutBounce(d-t, 0, c, d) + b;
	}

	public static float EaseOutBounce(float t, float b, float c, float d) {
        float u = t/d;
		if (u < (1/2.75)) {
			return c*(7.5625f*u*u) + b;
		} else if (u < (2/2.75)) {
            u -= 1.5f/2.75f;
			return c*(7.5625f*u*u + .75f) + b;
		} else if (u < (2.5/2.75)) {
            u -= 2.25f/2.75f;
			return c*(7.5625f*u*u + .9375f) + b;
		} else {
            u -= 2.625f/2.75f;
			return c*(7.5625f*u*u + .984375f) + b;
		}
	}

	public static float EaseInOutBounce(float t, float b, float c, float d) {
		if (t < d/2) return EaseInBounce (t*2, 0, c, d) * .5f + b;
		return EaseOutBounce (t*2-d, 0, c, d) * .5f + c*.5f + b;
	}
};

/*
 *
 * TERMS OF USE - EASING EQUATIONS
 *
 * Open source under the BSD License.
 *
 * Copyright © 2001 Robert Penner
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without modification,
 * are permitted provided that the following conditions are met:
 *
 * Redistributions of source code must retain the above copyright notice, this list of
 * conditions and the following disclaimer.
 * Redistributions in binary form must reproduce the above copyright notice, this list
 * of conditions and the following disclaimer in the documentation and/or other materials
 * provided with the distribution.
 *
 * Neither the name of the author nor the names of contributors may be used to endorse
 * or promote products derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
 *  COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 *  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE
 *  GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED
 * AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 *  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 */

