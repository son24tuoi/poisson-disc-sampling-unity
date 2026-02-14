using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace One.Utilities.Easing
{
    /// <summary>
    /// Used to interpret AnimationCurves as eases.
    /// </summary>
    internal class EaseCurve
    {
        readonly AnimationCurve _animCurve;

        // ***********************************************************************************
        // CONSTRUCTOR
        // ***********************************************************************************

        public EaseCurve(AnimationCurve animCurve)
        {
            _animCurve = animCurve;
        }

        // ===================================================================================
        // PUBLIC METHODS --------------------------------------------------------------------

        public float Evaluate(float time, float duration, float unusedOvershoot, float unusedPeriod)
        {
            float curveLen = _animCurve[_animCurve.length - 1].time;
            float timePerc = time / duration;
            float eval = _animCurve.Evaluate(timePerc * curveLen);
            return eval;
        }
    }
}
