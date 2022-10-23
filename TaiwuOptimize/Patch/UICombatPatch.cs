using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DG.Tweening;
using GameData.Domains;
using GameData.Domains.Combat;
using GameData.GameDataBridge;
using HarmonyLib;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace TaiwuOptimize.Patch
{
    public class UICombatPatch
    {
        public static readonly ushort CombatId = 
            (ushort)AccessTools.Field(typeof(DomainHelper.DomainIds), "Combat").GetValue(null);
        public static readonly ushort MethodStartFightId =
            (ushort)AccessTools.Field(typeof(CombatDomainHelper.MethodIds), "StartCombat").GetValue(null);
        public static readonly ushort DataIdTimeScale =
            (ushort)AccessTools.Field(typeof(CombatDomainHelper.DataIds), "TimeScale").GetValue(null);
        
        [HarmonyPatch(typeof(UI_Combat), "Update")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            int codeLine = 0;
            foreach (var codeInstruction in instructions)
            {
                if (codeLine == 11 && codeInstruction.opcode == OpCodes.Stloc_0)
                {
                    yield return codeInstruction;
                    yield return new CodeInstruction(OpCodes.Ldc_I4_1);
                    yield return new CodeInstruction(OpCodes.Stloc_0);
                    continue;
                }
                codeLine++;
                yield return codeInstruction;
            }
        }

        [HarmonyPatch(typeof(UI_Combat), "OnNotifyGameData")]
        [HarmonyPrefix]
        public static void OnNotifyGameDataPrefix(UI_Combat __instance,ref List<NotificationWrapper> notifications)
        {
            for (var index = 0; index < notifications.Count; index++)
            {
                var notification = notifications[index];
                var notifyBody = notification.Notification;
                if (notifyBody.Type == 1
                    && notifyBody.DomainId == CombatId
                    && notifyBody.MethodId == MethodStartFightId)
                {
                    if (ModMain.EnableQuickEnterCombat)
                    {
                        notifications.RemoveAt(index);
                        index--;
                    }
                }
                else if(notifyBody.Type == 0
                         && notifyBody.Uid.DomainId == CombatId
                         && notifyBody.Uid.DataId == DataIdTimeScale)
                {
                    __instance.StartCoroutine(DelaySetTimeScale(__instance));
                }
            }
        }

        private static IEnumerator DelaySetTimeScale(UI_Combat __instance)
        {
            yield return null;
            var useItem = (bool)AccessTools.Field(typeof(UI_Combat), "_selectingUseItem")
                .GetValue(__instance);
            if (!useItem)
            {
                var timeScale = (float)AccessTools.Field(typeof(UI_Combat), "_realTimeScale")
                    .GetValue(__instance);
                AccessTools.Method(typeof(UI_Combat), "UpdateTimeScale")
                    .Invoke(__instance, new object[] {timeScale});
                Debug.Log($"Set TimeScale ; {timeScale}");
            }
        }

        /// <summary>
        /// 最后一个闭包
        /// </summary>
        [HarmonyPatch]
        public class UICombatClosure1
        {
            [HarmonyTargetMethod]
            public static MethodBase TargetMethod()
            {
                var methods = 
                    AccessTools.GetDeclaredMethods(typeof(UI_Combat))
                        .Where(method => method.Name.Contains("<OnInit>"))
                        .ToList();
                return methods[methods.Count - 1];
            }
            
            [HarmonyPrefix]
            public static bool OnInitSpecialPrefix(UI_Combat __instance)
            {
                var tips = __instance.CGet<CanvasGroup>("CombatBeginTips");
                tips.gameObject.SetActive(false);
                if (ModMain.EnableQuickEnterCombat)
                {
                    return false;
                }

                return true;
            }
        }
        
        /// <summary>
        /// 倒数第二个闭包
        /// </summary>
        [HarmonyPatch]
        public class UICombatClosure2
        {
            [HarmonyTargetMethod]
            public static MethodBase TargetMethod()
            {
                var methods = 
                    AccessTools.GetDeclaredMethods(typeof(UI_Combat))
                        .Where(method => method.Name.Contains("<OnInit>"))
                        .ToList();
                return methods[methods.Count - 2];
            }
            
            [HarmonyPrefix]
            public static void OnInitPostfix(UI_Combat __instance)
            {
                __instance.StartCoroutine(OnStartFight(__instance));
            }
            
            public static IEnumerator OnStartFight(UI_Combat __instance)
            {
                yield return new WaitForSeconds(1f);
                if (ModMain.EnableQuickEnterCombat)
                {
                    var traverse = new Traverse(__instance);
                    GameDataBridge.AddMethodCall(__instance.Element.GameDataListenerId, 8, 38);
                    bool autoCombat = traverse.Field<GlobalSettings>("_settingData").Value.AutoCombat;
                    if (autoCombat)
                    {
                        SetAutoFight(__instance ,true);
                    }
                    float combatSpeed = traverse.Field<GlobalSettings>("_settingData").Value.CombatSpeed;
                    AccessTools.Method(typeof(UI_Combat), "SetDisplayTimeScale").Invoke(__instance,
                        new object[] { combatSpeed, false });
                    var raycaster = __instance.CGet<CanvasGroup>("CombatBeginTips").GetComponent<GraphicRaycaster>();
                    raycaster.enabled = false;
                }
                else
                {
                    var raycaster = __instance.CGet<CanvasGroup>("CombatBeginTips").GetComponent<GraphicRaycaster>();
                    raycaster.enabled = true;
                }
            }

            private static void SetAutoFight(UI_Combat __instance, bool autoCombat)
            {
                var traverse = new Traverse(__instance);
                traverse.Field("_autoCombat").SetValue(autoCombat);
                AccessTools.Method(typeof(UI_Combat), "UpdateAutoFightMark").Invoke(__instance, 
                    new object[] {autoCombat, false});
                GameDataBridge.AddMethodCall(__instance.Element.GameDataListenerId, 8, 40, autoCombat);
            }
        }
    }
}