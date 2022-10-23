using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace TaiwuOptimize
{
    public static class Utils
    {
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            var component = go.GetComponent<T>();
            if (component == null)
            {
                component = go.AddComponent<T>();
            }
            return component;
        }
        
        public static TweenerCore<float, float, FloatOptions> DOFade(this CanvasGroup canvasGroup, float endValue, 
            float duration,bool snapping = false)
        {
            var tweenCore = DOTween
                .To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, endValue, duration);
            tweenCore
                .SetOptions(snapping)
                .SetTarget(canvasGroup);
            
            return tweenCore;
        }
        
        public static void SetLayerRecursively(this GameObject go, int layer)
        {
            go.layer = layer;
            foreach (Transform child in go.transform)
            {
                child.gameObject.SetLayerRecursively(layer);
            }
        }
    }
}