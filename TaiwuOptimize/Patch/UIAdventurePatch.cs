using System;
using System.Collections;
using GameData.Domains.Adventure;
using GameData.GameDataBridge;
using HarmonyLib;
using UnityEngine;

namespace TaiwuOptimize.Patch
{
    public class UIAdventurePatch
    {
        [HarmonyPatch(typeof(UI_AdventureInfo), "AddTravelRecord")]
        [HarmonyPrefix]
        public static void Patch(UI_AdventureInfo __instance)
        {
            if (ModMain.EnableQuickAdventure)
            {
                UI_AdventureInfo.MoveRealDuration = 2f / UI_AdventureInfo._totalSpeed / ModMain.AdventureMoveSpeedScale;
            }
        }

        [HarmonyPatch(typeof(UI_Adventure), "DoTrunkUnfoldAnim")]
        [HarmonyPrefix]
        public static void Patch(UI_Adventure __instance)
        {
            UI_Adventure.UnfoldAnimationTimeScale = 1f / ModMain.AdventureUnfoldAnimationTimeScale;
        }
    }
}