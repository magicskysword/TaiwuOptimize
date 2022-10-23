using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameData.Domains.Adventure;
using HarmonyLib;
using UnityEngine;

namespace TaiwuOptimize.Patch
{
    public class UIAdventurePatch
    {
        [HarmonyPatch(typeof(UI_Adventure), "DoMove")]
        [HarmonyPrefix]
        public static bool Patch(UI_Adventure __instance,bool switchBranch, ref IEnumerator __result)
        {
            UI_Adventure.DoMoveAnimationTimeScale = 0.9f / ModMain.AdventureMoveSpeedScale;
            if(ModMain.EnableQuickAdventure)
            {
                __result = DoMoveEx(__instance, switchBranch);
                return false;
            }
            
            return true;
        }
        
        [HarmonyPatch(typeof(UI_Adventure), "DoTrunkUnfoldAnim")]
        [HarmonyPrefix]
        public static void Patch(UI_Adventure __instance)
        {
            UI_Adventure.UnfoldAnimationTimeScale = 1f / ModMain.AdventureUnfoldAnimationTimeScale;
        }

        private static IEnumerator DoMoveEx(UI_Adventure __instance, bool switchBranch)
        {
            var traverse = Traverse.Create(__instance);
            
            var pauseField = traverse.Field<bool>("_pause");
            var playerPosField = traverse.Field<int>("_playerPos");
            var adventureStateField = traverse.Field<sbyte>("_adventureState");
            var curStyleField = traverse.Field<sbyte>("_curStyle");
            var testField = traverse.Field<bool>("_test");
            var moveAnimPlaying = traverse.Field<bool>("_moveAnimPlaying");

            var setToScreenCenterMethod = AccessTools.Method(typeof(UI_Adventure), "SetToScreenCenter");
            
            var playerRefers = traverse.Field<Refers>("_playerRefers").Value;
            var nodeRefersDict = traverse.Field<Dictionary<int, Refers>>("_nodeRefersDict").Value;
            var indicateLineVertices = traverse.Field<List<Vector2>>("_indicateLineVertices").Value;
            var mapPointsDict = traverse.Field<Dictionary<int, AdventureMapPoint>>("_mapPointsDict").Value;
            var prevTerrains = traverse.Field<List<string>>("_prevTerrains").Value;
            
            while (pauseField.Value)
                yield return null;

            traverse.Field<bool>("_moveAnimPlaying").Value = true;
            AudioManager.Instance.PlaySound("ui_adventure_foot");
            float animTime = 0.0f;
            float moveDuration = 0.5f * UI_Adventure.DoMoveAnimationTimeScale;
            Transform moveTransform = playerRefers.transform;
            Vector2 srcPos = moveTransform.localPosition;
            Vector2 dstPos =
                playerRefers.transform.parent.InverseTransformPoint(nodeRefersDict[playerPosField.Value]
                    .transform.position);
            CImage lineDrawer = __instance.LayerConnct.GetComponent<CImage>();
            float updateLineOffset = 0.05f;
            if (adventureStateField.Value != 4 
                && (indicateLineVertices.Count <= 1 
                    || (indicateLineVertices[indicateLineVertices.Count - 1] - srcPos).magnitude > 20.0))
            {
                indicateLineVertices.Add(srcPos);
                lineDrawer.SetVerticesDirty();
            }

            while (animTime < (double)moveDuration)
            {
                if (!pauseField.Value)
                {
                    animTime += Time.deltaTime;
                    moveTransform.localPosition =
                        Vector3.Lerp(srcPos, dstPos, animTime / moveDuration);
                    updateLineOffset -= Time.deltaTime;
                    if (updateLineOffset <= 0.0 && indicateLineVertices.Count > 0)
                    {
                        updateLineOffset = 0.05f;
                        float nextOffsetT = Math.Min((animTime + 0.05f) / moveDuration, 1f);
                        if (nextOffsetT < 1.0)
                            indicateLineVertices[indicateLineVertices.Count - 1] =
                                Vector3.Lerp(srcPos, dstPos, nextOffsetT);
                        lineDrawer.SetVerticesDirty();
                    }
                }

                if (ModMain.EnableAdventureCameraFollow)
                {
                    setToScreenCenterMethod.Invoke(__instance,new object[] {playerRefers.transform.position, -1, null});
                }
                yield return null;
            }

            if (adventureStateField.Value != 4 && indicateLineVertices.Count > 0)
            {
                indicateLineVertices.RemoveAt(indicateLineVertices.Count - 1);
                lineDrawer.SetVerticesDirty();
            }
            
            AccessTools.Method(typeof(UI_Adventure), "SetFocusGrid")
                .Invoke(__instance, new object[] {playerPosField.Value});
            if (ModMain.EnableAdventureCameraFollow)
            {
                setToScreenCenterMethod.Invoke(__instance,new object[]{playerRefers.transform.position,-1, null});
            }
            else
            {
                setToScreenCenterMethod.Invoke(__instance,new object[]{playerRefers.transform.position,
                    0.5f * UI_Adventure.DoMoveAnimationTimeScale, null});
            }
            // traverse.Method("SetToScreenCenter").GetValue(playerRefers.transform.position,
            //     0.5f * UI_Adventure.DoMoveAnimationTimeScale);

            animTime = 0.0f;
            moveDuration = 0.5f * UI_Adventure.DoMoveAnimationTimeScale;
            Color srcColor = Color.clear;
            Color dstColor = Color.white;
            Refers refers = nodeRefersDict[playerPosField.Value];
            AdventureMapPoint node = mapPointsDict[playerPosField.Value];
            CImage bgImg = refers.CGet<CImage>("BgShape");
            if (node.NodeType == -1 || node.NodeType == 0)
            {
                //__instance.RefreshBgShape(playerPosField.Value, refers, node);
                AccessTools.Method(typeof(UI_Adventure), "RefreshBgShape")
                    .Invoke(__instance, new object[] {playerPosField.Value, refers, node});
                refers.CGet<CImage>("FlatTerrainImg").gameObject.SetActive(false);
                CImage terrain = refers.CGet<CImage>("TerrainImg");
                terrain.color = Color.clear;
                terrain.gameObject.SetActive(curStyleField.Value != 1);
                prevTerrains.Add(terrain.sprite.name);
                float progressValue = 0f;
                DOTween.To(() => progressValue, x =>
                {
                    progressValue = x;
                    terrain.SetColor(Color.Lerp(srcColor, dstColor, progressValue));
                }, 1f, moveDuration);
                // while (animTime < (double)moveDuration)
                // {
                //     if (!pauseField.Value)
                //     {
                //         animTime += Time.deltaTime;
                //         terrain.SetColor(Color.Lerp(srcColor, dstColor, animTime / moveDuration));
                //     }
                //     yield return null;
                // }
            }
            else
            {
                bgImg.SetSprite("adventure_01_gn_gezi_6");
                refers.CGet<CImage>("FlatTerrainImg").gameObject.SetActive(false);
                if (!testField.Value && adventureStateField.Value != 4)
                {
                    refers.CGet<GameObject>("TerrainImgs").SetActive(true);
                    for (int i = 0; i < 3; ++i)
                    {
                        int index = UnityEngine.Random.Range(0, prevTerrains.Count);
                        CImage terrain = refers.CGet<CImage>(string.Format("TerrainImg_{0}", i));
                        terrain.SetSprite(prevTerrains[index]);
                        terrain.SetNativeSize();
                        terrain.color = Color.clear;
                        terrain = null;
                    }

                    prevTerrains.Clear();
                    for (int i = 0; i < 3; ++i)
                    {
                        CImage terrain = refers.CGet<CImage>(string.Format("TerrainImg_{0}", i));
                        float progressValue = 0f;
                        DOTween.To(() => progressValue, x =>
                        {
                            progressValue = x;
                            terrain.SetColor(Color.Lerp(srcColor, dstColor, progressValue));
                        }, 1f, moveDuration);
                    }
                    
                    // while (animTime < (double)moveDuration)
                    // {
                    //     if (!pauseField.Value)
                    //     {
                    //         animTime += Time.deltaTime;
                    //         for (int i = 0; i < 3; ++i)
                    //         {
                    //             CImage terrain = refers.CGet<CImage>(string.Format("TerrainImg_{0}", i));
                    //             terrain.SetColor(Color.Lerp(srcColor, dstColor, animTime / moveDuration));
                    //             terrain = null;
                    //         }
                    //     }
                    //
                    //     yield return null;
                    // }
                }
            }

            moveAnimPlaying.Value = false;
            if (switchBranch && adventureStateField.Value == 4)
                __instance.ClearCurrentBranch();
            else if (!testField.Value)
            {
                GameData.GameDataBridge.GameDataBridge.AddMethodCall(__instance.Element.GameDataListenerId, 10,
                    8);
                //yield return new WaitForSeconds(0.2f);
            }
        }
    }
}