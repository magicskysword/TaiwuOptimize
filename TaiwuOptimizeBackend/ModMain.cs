using System;
using GameData.Domains;
using HarmonyLib;
using NLog;
using TaiwuModdingLib.Core.Plugin;
using TaiwuOptimizeBackend.Patch;

namespace TaiwuOptimizeBackend
{
    [PluginConfig("太吾优化", "剑圣(skyswordkill)", "1.1.0")]
    public class ModMain : TaiwuRemakePlugin
    {
        public static bool DisableSecretInformationSystem = false;
        
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        private Harmony _harmony;

        public override void Initialize()
        {
            _harmony = new Harmony("TaiwuOptimizeBackend");
        }

        public override void OnModSettingUpdate()
        {
            DomainManager.Mod.GetSetting(ModIdStr, "Bool_DisableSecretInformationSystem", 
                ref DisableSecretInformationSystem);
            
            
            _harmony.UnpatchSelf();
            if (typeof(GameData.Domains.Information.Collection.SecretInformationCollection)
                    .GetField("FastEnough") == null 
                && DisableSecretInformationSystem) 
                _harmony.PatchAll(typeof(InformationPatch));
        }

        public override void Dispose()
        {
            _harmony.UnpatchSelf();
            _harmony = null;
        }
        
        public static void Log(object message)
        {
            Logger.Info(message);
        }
    }
}