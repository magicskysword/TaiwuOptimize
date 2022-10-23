using System.Collections.Generic;
using System.Reflection.Emit;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using HarmonyLib;
using UnityEngine;

namespace TaiwuOptimize.Patch
{
    public class UIEventWindowPatch
    {
        [HarmonyPatch(typeof(UI_EventWindow), "OnInit")]
        [HarmonyPostfix]
        public static void OnInitPostfix(UI_EventWindow __instance)
        {
            __instance.WindowAnimDuration = ModMain.EventWindowFadeTime;
        }
        
        [HarmonyPatch(typeof(UI_EventWindow), "AnimEventWindowIn")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> AnimEventWindowInTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var target = AccessTools.Method(typeof(ShortcutExtensions), "DOLocalMove");
            var newCall = CodeInstruction.Call(typeof(UIEventWindowPatch), "FadeInEx");
            foreach (var codeInstruction in instructions)
            {
                if (codeInstruction.Calls(target))
                {
                    yield return newCall;
                }
                else
                {
                    yield return codeInstruction;
                }
            }
        }
        
        [HarmonyPatch(typeof(UI_EventWindow), "AnimEventWindowOut")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> AnimEventWindowOutTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var target = AccessTools.Method(typeof(ShortcutExtensions), "DOLocalMove");
            var newCall = CodeInstruction.Call(typeof(UIEventWindowPatch), "FadeOutEx");
            foreach (var codeInstruction in instructions)
            {
                if (codeInstruction.Calls(target))
                {
                    yield return newCall;
                }
                else
                {
                    yield return codeInstruction;
                }
            }
        }

        public static TweenerCore<Vector3, Vector3, VectorOptions> FadeInEx(Transform target, Vector3 endValue,
            float duration, bool snapping = false)
        {
            if (ModMain.EnableEventWindowFade)
            {
                var canvasGroup = target.gameObject.GetOrAddComponent<CanvasGroup>();
                canvasGroup.alpha = 0;
                canvasGroup.DOFade(1, duration, snapping);
                target.localPosition = Vector3.zero;
            }
            return target.DOLocalMove(endValue, duration, snapping);
        }
        
        public static TweenerCore<Vector3, Vector3, VectorOptions> FadeOutEx(Transform target, Vector3 endValue,
            float duration, bool snapping = false)
        {
            if(ModMain.EnableEventWindowFade)
            {
                var canvasGroup = target.gameObject.GetOrAddComponent<CanvasGroup>();
                canvasGroup.DOFade(0, duration, snapping);
                return target.DOLocalMove(Vector3.zero, duration, snapping);
            }

            return target.DOLocalMove(endValue, duration, snapping);
        }
    }
}