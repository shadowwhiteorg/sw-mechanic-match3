using UnityEngine;
using System;
using _Game.Enums;

namespace _Game.Utils
{
    public static class TweenExtensions
    {
        public static Coroutine TweenPosition(this Transform transform, Vector3 to, float duration, Ease ease = Ease.Linear, Action onComplete = null)
            => Tween.Position(transform, to, duration, ease, onComplete);

        public static Coroutine TweenScale(this Transform transform, Vector3 to, float duration, Ease ease = Ease.Linear, Action onComplete = null)
            => Tween.Scale(transform, to, duration, ease, onComplete);
    }
}