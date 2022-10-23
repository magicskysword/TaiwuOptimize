using System;
using DG.Tweening;
using GameData.Domains.Combat;
using HarmonyLib;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace TaiwuOptimize.Patch
{
    public class UICombatResultPatch
    {
        [HarmonyPatch(typeof(UI_CombatResult), "OnInit")]
        [HarmonyPostfix]
        public static void OnInitPostfix(UI_CombatResult __instance)
        {
            var resultAni = __instance.CGet<SkeletonGraphic>("ResultAni");
            if (Math.Abs(ModMain.CombatResultAnimScale - 1f) > 0.1f)
            {
                sbyte combatResult = Traverse.Create(__instance).Field("_combatResult").GetValue<sbyte>();
                bool win = CombatResultType.IsPlayerWin(combatResult);
                resultAni.timeScale = ModMain.CombatResultAnimScale;
                AudioManager.Instance.SetSoundTimeScale(ModMain.CombatResultAnimScale);
                resultAni.AnimationState.SetAnimation(0, win ? "combat_win" : "combat_lose", false);
                var mainWindow = __instance.CGet<CanvasGroup>("MainWindow");
                var pointerMask = __instance.CGet<GameObject>("PointerMask");
                CButton closeBtn = __instance.CGet<CButton>("Close");
                float aniTime = (win ? 4f : 3.33f) / ModMain.CombatResultAnimScale;
                var btnCanvas = closeBtn.GetComponent<CanvasGroup>();
                mainWindow.alpha = 0f;
                mainWindow.DOKill();
                mainWindow.DOFade(1f, 0.5f)
                    .SetDelay(aniTime)
                    .OnComplete(() =>
                {
                    pointerMask.SetActive(false);
                });
                closeBtn.interactable = false;
                btnCanvas.DOKill();
                btnCanvas.alpha = 0f;
                btnCanvas.DOFade(1f, 0.5f)
                    .SetDelay(aniTime)
                    .OnComplete(() =>
                {
                    closeBtn.interactable = true;
                });
                //resultAni.AnimationState.TimeScale = 3f;
            }
            else
            {
                resultAni.timeScale = 1f;
                //resultAni.AnimationState.TimeScale = 1f;
            }
        }
    }
}