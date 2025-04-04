using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameData.Domains;
using GameData.Domains.Map;
using GameData.GameDataBridge;
using HarmonyLib;
using TaiwuModdingLib.Core.Utils;
using UnityEngine;

namespace TaiwuOptimize.Patch
{
    public class UIWorldMapPatch
    {
        public static ushort DomainIdMap = DomainHelper.DomainName2DomainId["Map"];
        public static ushort MapMethodIdMoveFinish = MapDomainHelper.MethodName2MethodId["MoveFinish"];
        
        [HarmonyPatch(typeof(UI_Worldmap), "PlayMoveAni")]
        [HarmonyPrefix]
        public static bool PlayMoveAni_Prefix(UI_Worldmap __instance, short fromBlockId, short toBlockId)
        {
            if (ModMain.EnableQuickWorldMapMove)
            {
                PlayMoveAniEx(__instance, fromBlockId, toBlockId);
                return false;
            }

            return true;
        }

        public static void PlayMoveAniEx(UI_Worldmap __instance, short fromBlockId, short toBlockId)
        {
            if (__instance._teleportMoving)
            {
                __instance._teleportMoving = false;
                __instance.SetPlayerLocationWithRefreshAllBlockVisibility(new Location(__instance._mapModel.CurrentAreaId, toBlockId));
                __instance.OnMoveAniComplete(fromBlockId);
                __instance.RefreshAllBlockVisibility();
            }
            else
                __instance.StartCoroutine(CoPlayMoveAnim(__instance ,fromBlockId, toBlockId));
        }

        // private static void PlayMoveAniExOld(UI_Worldmap __instance, short fromBlockId, short toBlockId)
        // {
        //     if (__instance._teleportMoving)
        //     {
        //         __instance._teleportMoving = false;
        //         __instance.SetPlayerLocationWithRefreshAllBlockVisibility(new Location(__instance._mapModel.CurrentAreaId, toBlockId));
        //         __instance.OnMoveAniComplete(fromBlockId);
        //         __instance.RefreshAllBlockVisibilityWithForce();
        //     }
        //     else
        //         __instance.StartCoroutine(CoPlayMoveAnim(__instance, fromBlockId, toBlockId));
        // }
        
        public static IEnumerator CoPlayMoveAnim(UI_Worldmap __instance, short fromBlockId, short toBlockId)
        {
            __instance._mapModel.ChangeTaiwuMoveState(WorldMapModel.MoveState.PerformMove);
            Location fromLocation = new Location(__instance.PlayerAtBlock.AreaId, fromBlockId);
            Location toLocation = new Location(__instance.PlayerAtBlock.AreaId, toBlockId);
            bool eventPerforming = UIElement.EventWindow.Exist;
            if (__instance._mapModel.ShowingAreaId != __instance.PlayerAtBlock.AreaId)
                __instance.SetShowingArea(__instance.PlayerAtBlock.AreaId);
            
            float duration = UI_Worldmap.MoveStepTime / ModMain.WorldMapMoveSpeedScale;
            var tweener = DOVirtual.Float(0.0f, 1f, duration, stepVal => __instance.SetPlayerLocationWithRefreshAllBlockVisibility(fromLocation, toLocation, stepVal, false));
            if (ModMain.EnableLinerWorldMapMove)
            {
                tweener.SetEase(Ease.Linear);
            }
            
            yield return new WaitForSeconds(duration);
            if (eventPerforming || !__instance._movingByController)
            {
                GameDataBridge.AddMethodCall(-1, 2, 8, fromLocation, toLocation);
                __instance.ClearMovePath();
            }
            __instance.OnMoveAniComplete(fromBlockId);
        }
        
        // public static IEnumerator CoPlayMoveAnimOld(UI_Worldmap __instance, short fromBlockId, short toBlockId)
        // {
        //     __instance._mapModel.ChangeTaiwuMoveState(WorldMapModel.MoveState.PerformMove);
        //     Location fromLocation = new Location(__instance.PlayerAtBlock.AreaId, fromBlockId);
        //     Location toLocation = new Location(__instance.PlayerAtBlock.AreaId, toBlockId);
        //     bool eventPerforming = UIElement.EventWindow.Exist;
        //     if (__instance._mapModel.ShowingAreaId != __instance.PlayerAtBlock.AreaId)
        //         __instance.SetShowingArea(__instance.PlayerAtBlock.AreaId);
        //     UI_Worldmap._waitingUpdateBlockSet.UnionWith(UI_Worldmap._normalColorBlockSet);
        //     float duration = UI_Worldmap.MoveStepTime / ModMain.WorldMapMoveSpeedScale;
        //     
        //     var tweener = DOVirtual.Float(0.0f, 1f, duration, stepVal => __instance.SetPlayerLocationWithRefreshAllBlockVisibility(fromLocation, toLocation, stepVal, false));
        //     if (ModMain.EnableLinerWorldMapMove)
        //     {
        //         tweener.SetEase(Ease.Linear);
        //     }
        //
        //     yield return new WaitForSeconds(duration);
        //
        //     UI_Worldmap._waitingUpdateBlockSet.UnionWith(UI_Worldmap._normalColorBlockSet);
        //     foreach (Location location in UI_Worldmap._waitingUpdateBlockSet)
        //     {
        //         MapBlockData block = __instance._mapModel.GetBlockData(location);
        //         __instance.ProcessBlock(block);
        //         block = null;   
        //     }
        //     UI_Worldmap._waitingUpdateBlockSet.Clear();
        //     if (eventPerforming || !__instance._movingByController)
        //     {
        //         GameDataBridge.AddMethodCall(-1, 2, 8, fromLocation, toLocation);
        //         __instance.ClearMovePath();
        //     }
        //     
        //     __instance.OnMoveAniComplete(fromBlockId);
        // }
    }
}