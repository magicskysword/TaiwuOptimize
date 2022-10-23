using System;
using DG.Tweening;
using GameData.Domains;
using GameData.Domains.Map;
using GameData.GameDataBridge;
using HarmonyLib;
using TaiwuModdingLib.Core.Utils;

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

        private static void PlayMoveAniEx(UI_Worldmap __instance, short fromBlockId, short toBlockId)
        {
            var traverse = Traverse.Create(__instance);
            var teleportMovingField = traverse.Field<bool>("_teleportMoving");
            var mapModelField = traverse.Field<WorldMapModel>("_mapModel");
            var movingByControllerField = traverse.Field<bool>("_movingByController");
            
            var onMoveAniCompleteMethod = AccessTools.Method(typeof(UI_Worldmap), "OnMoveAniComplete");
            var setShowingAreaMethod = AccessTools.Method(typeof(UI_Worldmap), "SetShowingArea");
            var clearMovePathMethod = AccessTools.Method(typeof(UI_Worldmap), "ClearMovePath");
            var selectBlockMethod = AccessTools.Method(typeof(UI_Worldmap), "SelectBlock");

            if (teleportMovingField.Value)
            {
                teleportMovingField.Value = false;
                __instance.SetPlayerLocation(new Location(mapModelField.Value.CurrentAreaId, toBlockId));
                onMoveAniCompleteMethod.Invoke(__instance, new object[] {fromBlockId});
                __instance.RefreshAllBlockVisibility();
            }
            else
            {
                Location fromLocation = new Location(__instance.PlayerAtBlock.AreaId, fromBlockId);
                Location toLocation = new Location(__instance.PlayerAtBlock.AreaId, toBlockId);
                bool eventPerforming = UIElement.EventWindow.Exist;
                if (mapModelField.Value.ShowingAreaId != __instance.PlayerAtBlock.AreaId)
                {
                    setShowingAreaMethod.Invoke(__instance, new object[] {__instance.PlayerAtBlock.AreaId});
                }
                __instance.MoveCameraTo(fromLocation, true,
                    () =>
                    {
                        var tweener = DOVirtual.Float(0.0f, 1f, 0.4f / ModMain.WorldMapMoveSpeedScale,
                                stepVal => 
                                    __instance.SetPlayerLocation(fromLocation, toLocation, stepVal))
                            .OnComplete(() =>
                            {
                                if (eventPerforming || !movingByControllerField.Value)
                                {
                                    GameDataBridge.AddMethodCall(-1, DomainIdMap, MapMethodIdMoveFinish, fromLocation, toLocation);
                                    clearMovePathMethod.Invoke(__instance, new object[] { });
                                    selectBlockMethod.Invoke(__instance, new object[]
                                    {
                                        mapModelField.Value.GetBlockData(toLocation)
                                    });
                                }
                                    
                                onMoveAniCompleteMethod.Invoke(__instance, new object[] {fromBlockId});
                            });
                        if (ModMain.EnableLinerWorldMapMove)
                        {
                            tweener.SetEase(Ease.Linear);
                        }
                    }
                );
            }
        }
    }
}