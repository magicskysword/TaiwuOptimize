using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using GameData.Domains.Character.Display;
using GameData.Utilities;
using HarmonyLib;
using TMPro;
using UICommon.Character.Avatar;
using UnityEngine;

namespace TaiwuOptimize.Patch
{
    public class UISettlementInformationPatch
    {
        [HarmonyPatch(typeof(UI_SettlementInformation), "HandleSettlementMembersProcess")]
        [HarmonyPrefix]
        public static bool HandleSettlementMembersProcess_Prefix(UI_SettlementInformation __instance,
            int offset, RawDataPool dataPool, List<CharacterDisplayData> chars, ref IEnumerator __result)
        {
            if (ModMain.EnableSettlementOptimize)
            {
                __result = HandleSettlementMembersProcessEx(__instance, offset, dataPool, chars);
                return false;
            }
            
            return true;
        }

        private static IEnumerator HandleSettlementMembersProcessEx(UI_SettlementInformation __instance,
            int offset, RawDataPool dataPool, List<CharacterDisplayData> chars)
        {
            var thisUi = Traverse.Create(__instance);
            
            thisUi.Field("_CurrentGradeIndex").SetValue(-1);
            foreach (object obj in __instance.CharRoot)
            {
                Transform charGroup = (Transform)obj;
                CImage image = charGroup.GetComponent<CImage>();
                bool flag = image != null;
                if (flag)
                {
                    image.enabled = false;
                }
            }
            
            __instance.CharRoot.gameObject.SetActive(false);
            var list = new List<(CharacterDisplayData data, Refers refers,bool isKnown)>();
            var stopwatch = Stopwatch.StartNew();
            foreach (CharacterDisplayData one in chars)
            {
                sbyte grade = one.OrgInfo.Grade;
                Transform panel = __instance.CharRoot.GetChild(8 - grade);
                GameObject charObj = PoolManager.GetObject("UI_SettlementInformationCharInfoObject");
                Refers charRefers = charObj.transform.GetComponent<Refers>();
                charObj.SetActive(true);
                charObj.transform.SetParent(panel, false);
                bool isKnown = one.FavorabilityToTaiwu != short.MinValue ||
                               one.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
                list.Add((one, charRefers, isKnown));
                CImage image = panel.GetComponent<CImage>();
                if (image != null)
                {
                    image.enabled = true;
                }
                

                charRefers.CGet<TextMeshProUGUI>("NameText").text = (isKnown
                    ? NameCenter.GetCharMonasticTitleOrNameByDisplayData(one, false, false)
                    : "? ? ?");
                charRefers.CGet<TextMeshProUGUI>("PowerText").text = $"{one.InfluencePower}";
                
                if(stopwatch.ElapsedMilliseconds > 33)
                {
                    yield return null;
                    stopwatch.Restart();
                }
            }
            
            yield return null;
            __instance.CharRoot.gameObject.SetActive(true);
            foreach (var tuple in list)
            {
                var data = tuple.data;
                var charRefers = tuple.refers;
                var isKnown = tuple.isKnown;
                
                if (isKnown)
                {
                    Avatar avatar =
                        charRefers.CGet<Avatar>("Avatar");
                    avatar.gameObject.SetActive(true);
                    avatar.Refresh(data.AvatarRelatedData);
                    charRefers.CGet<GameObject>("Unknow").SetActive(false);
                }
                else
                {
                    charRefers.CGet<Avatar>("Avatar").gameObject.SetActive(false);
                    charRefers.CGet<GameObject>("Unknow").gameObject.SetActive(true);
                }
            }
            
            //__instance.CharRoot.gameObject.SetActive(true);
            __instance.OnScale(__instance.CharContent.localScale);
            __instance.OnSetScrollOffset(Vector2.zero);
            thisUi.Field("_charInfoEnteringCoroutine").SetValue(null);
            yield break;
        }
    }
}