using DG.Tweening;
using UnityEngine;

namespace Miograph.Animation
{
    [CreateAssetMenu(fileName = "AnimSettings", menuName = "AnimationSettings")]
    public class AnimSettingsObject : ScriptableObject
    {
        public float Duration;
        public Ease Ease;
        public float Delay;
        public bool IsUseCustomCurve;
        public AnimationCurve AnimationCurve;
    }
}
