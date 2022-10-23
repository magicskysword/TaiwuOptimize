using HarmonyLib;
using TaiwuModdingLib.Core.Plugin;
using TaiwuOptimize.Patch;
using UnityEngine;

namespace TaiwuOptimize
{
    [PluginConfig("滚动优化", "剑圣(skyswordkill)", "1.0.0")]
    public class ModMain : TaiwuRemakePlugin
    {
        private Harmony _harmony;
        public static ModMono ModMono;
        
        public static float AvatarFadeInTime = 0.3f;
        public static float EventWindowFadeTime = 0.3f; 
        
        public static bool EnableAvatarDelayRefresh = true;
        public static bool EnableSettlementOptimize = true;
        public static bool EnableEventWindowFade = true;
        public static bool EnableQuickEnterCombat = true;
        public static bool EnableQuickAdventure = true;
        public static bool EnableAdventureCameraFollow = true;
        public static bool EnableQuickWorldMapMove = true;
        public static bool EnableLinerWorldMapMove = true;
        
        public static float CombatResultAnimScale = 1f;
        public static float AdventureMoveSpeedScale = 1f;
        public static float AdventureUnfoldAnimationTimeScale = 1f;
        public static float WorldMapMoveSpeedScale = 1f;
        


        public override void Initialize()
        {
            _harmony = new Harmony("skyswordkill.taiwu.optimize");
            _harmony.PatchAll(typeof(AvatarPatch));
            _harmony.PatchAll(typeof(UISettlementInformationPatch));
            _harmony.PatchAll(typeof(UIEventWindowPatch));
            _harmony.PatchAll(typeof(UICombatPatch));
            _harmony.PatchAll(typeof(UICombatPatch.UICombatClosure1));
            _harmony.PatchAll(typeof(UICombatPatch.UICombatClosure2));
            _harmony.PatchAll(typeof(UICombatResultPatch));
            _harmony.PatchAll(typeof(UIAdventurePatch));
            _harmony.PatchAll(typeof(UIWorldMapPatch));
            var gameObject = new GameObject("TaiwuOptimize");
            ModMono = gameObject.AddComponent<ModMono>();
            GameObject.DontDestroyOnLoad(gameObject);
        }

        public override void Dispose()
        {
            _harmony.UnpatchSelf();
            _harmony = null;
            GameObject.Destroy(ModMono.gameObject);
        }

        public override void OnModSettingUpdate()
        {
            int avatarFadeInType = 0;
            // 头像延迟加载与滚动列表
            ModManager.GetSetting(ModIdStr, "Bool_EnableAvatarDelayRefresh", ref EnableAvatarDelayRefresh);
            ModManager.GetSetting(ModIdStr, "Int_AvatarFadeInTimeType", ref avatarFadeInType);
            AvatarFadeInTime = BindFadeTime(avatarFadeInType);
            
            // 势力界面优化
            ModManager.GetSetting(ModIdStr, "Bool_EnableSettlementOptimize", ref EnableSettlementOptimize);
            
            // 对话框渐显
            ModManager.GetSetting(ModIdStr, "Bool_EnableEventWindowFade", ref EnableEventWindowFade);
            int eventWindowFadeType = 0;
            ModManager.GetSetting(ModIdStr, "Int_EventWindowFadeTimeType", ref eventWindowFadeType);
            
            // 快速进入战斗与战斗结算
            EventWindowFadeTime = BindFadeTime(eventWindowFadeType);
            ModManager.GetSetting(ModIdStr, "Bool_EnableQuickEnterCombat", ref EnableQuickEnterCombat);
            int combatResultAnimScaleType = 0;
            ModManager.GetSetting(ModIdStr, "Int_CombatResultAnimScaleType", ref combatResultAnimScaleType);
            CombatResultAnimScale = BindTimeScale(combatResultAnimScaleType);
            
            // 奇遇优化
            ModManager.GetSetting(ModIdStr, "Bool_EnableQuickAdventure", ref EnableQuickAdventure);
            ModManager.GetSetting(ModIdStr, "Bool_EnableAdventureCameraFollow", ref EnableAdventureCameraFollow);
            int adventureMoveSpeedScaleType = 0;
            ModManager.GetSetting(ModIdStr, "Int_AdventureMoveSpeedScaleType", ref adventureMoveSpeedScaleType);
            AdventureMoveSpeedScale = BindTimeScale(adventureMoveSpeedScaleType);
            int adventureUnfoldAnimationTimeScaleType = 0;
            ModManager.GetSetting(ModIdStr, "Int_AdventureUnfoldAnimationTimeScaleType", ref adventureUnfoldAnimationTimeScaleType);
            AdventureUnfoldAnimationTimeScale = BindTimeScale(adventureUnfoldAnimationTimeScaleType);
            
            // 世界地图优化
            ModManager.GetSetting(ModIdStr, "Bool_EnableQuickWorldMapMove", ref EnableQuickWorldMapMove);
            ModManager.GetSetting(ModIdStr, "Bool_EnableLinerWorldMapMove", ref EnableLinerWorldMapMove);
            int worldMapMoveSpeedScaleType = 0;
            ModManager.GetSetting(ModIdStr, "Int_WorldMapMoveSpeedScaleType", ref worldMapMoveSpeedScaleType);
            WorldMapMoveSpeedScale = BindTimeScale(worldMapMoveSpeedScaleType);
        }

        private float BindTimeScale(int adventureMoveSpeedScaleType)
        {
            switch (adventureMoveSpeedScaleType)
            {
                case 0:
                    return 1f;
                case 1:
                    return 1.25f;
                case 2:
                    return 1.5f;
                case 3:
                    return 1.75f;
                case 4:
                    return 2f;
                case 5:
                    return 3f;
                case 6:
                    return 4f;
                case 7:
                    return 5f;
                default:
                    return 1f;
            }
        }

        public float BindFadeTime(int fadeTimeType)
        {
            switch (fadeTimeType)
            {
                case 0:
                    return 0.3f;
                case 1:
                    return 0.5f;
                case 2:
                    return 1f;
                default:
                    return 0f;
            }
        }
    }
}