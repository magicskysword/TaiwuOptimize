using System.Collections;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace TaiwuOptimize.Patch
{
    public class UICombatPatch
    {
        public static TextMeshProUGUI _allyAttackRange;
        public static TextMeshProUGUI _enemyAttackRange;
        
        [HarmonyPatch(typeof(UI_Combat), "Awake")]
        [HarmonyPostfix]
        public static void AwakePostfix(UI_Combat __instance)
        {
            if (ModMain.EnableCombatRange)
            {
                var selfMobility = __instance._selfInfoChar.CGet<RectTransform>("Mobility");
                var enemyMobility = __instance._enemyInfoChar.CGet<RectTransform>("Mobility");
                var font = __instance._selfInfoChar.CGet<RectTransform>("OutOfAttackRangeTips")
                    .GetChild(0).GetComponent<TextMeshProUGUI>().font;
            
                _allyAttackRange = CreateAttackRange(selfMobility, font);
                _enemyAttackRange = CreateAttackRange(enemyMobility, font);
            }
        }

        private static TextMeshProUGUI CreateAttackRange(RectTransform parent, TMP_FontAsset fontAsset)
        {
            var attackRange = new GameObject("AttackRange");
            attackRange.transform.SetParent(parent);
            var rectTransform = attackRange.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(200, 50);
            rectTransform.anchoredPosition = new Vector2(0, -35);
            rectTransform.localScale = Vector3.one;
            
            var text = attackRange.AddComponent<TextMeshProUGUI>();
            text.font = fontAsset;
            text.fontSize = 24;
            text.color = new Color(0.7569f, 0.4118f, 0.1529f, 1);
            text.alignment = TextAlignmentOptions.Center;
            text.horizontalAlignment = HorizontalAlignmentOptions.Center;
            
            return text;
        }

        [HarmonyPatch(typeof(UI_Combat), "UpdateAttackRange")]
        [HarmonyPostfix]
        public static void UpdateAttackRange(UI_Combat __instance, bool isAlly)
        {
            var outerAndInnerShorts = isAlly ? __instance._selfAttackRange : __instance._enemyAttackRange;
            var range =
                $"{(float)(Mathf.Max(outerAndInnerShorts.Outer, 20) / 10.0):f1}" +
                $"  ——  " +
                $"{(float)(Mathf.Min(outerAndInnerShorts.Inner, 120) / 10.0):f1}";

            if (isAlly && _allyAttackRange != null)
            {
                _allyAttackRange.text = range;
            }
            else if (!isAlly && _enemyAttackRange != null)
            {
                _enemyAttackRange.text = range;
            }
        }
    }
}