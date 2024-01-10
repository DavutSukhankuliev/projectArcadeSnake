using System;
using DG.Tweening;
using UnityEngine;

namespace Miograph.Animation
{
    [Serializable]
    public class AnimSettings
    {
        public float Duration;
        public Ease Ease;
        public float Delay;
        public bool IsUseCustomCurve;
        public AnimationCurve AnimationCurve;

        public AnimSettings(float duation, Ease ease = Ease.Linear, float delay = 0, bool isUseCustomCurve = false,
            AnimationCurve animationCurve = null)
        {
            Duration = duation;
            Ease = ease;
            Delay = delay;
            IsUseCustomCurve = isUseCustomCurve;
            AnimationCurve = animationCurve;
        }

        public AnimSettings() : this(1f, Ease.Linear, 0f, false, null)
        {
            
        }

        public AnimSettings(float duration, AnimationCurve curve) : this(duration, DG.Tweening.Ease.Unset, 0f, true,
            curve)
        {
            
        }
    }
}