using System;
using System.Collections.Generic;
using HarmonyLib;
using TaiwuOptimize.Behaviour;
using UICommon.Character.Avatar;

namespace TaiwuOptimize.Patch
{
    public class AvatarPatch
    {
        [HarmonyPatch(typeof(Avatar), "Refresh", new Type[0])]
        [HarmonyPrefix]
        public static bool RefreshPrefix(Avatar __instance)
        {
            if(!ModMain.EnableAvatarDelayRefresh)
                return true;
            
            var record = __instance.GetComponent<AvatarRecord>();
            if (record == null)
            {
                record = __instance.gameObject.AddComponent<AvatarRecord>();
                record.Init();
            }

            record.HideAvatar();

            if (record.CanRefresh)
            {
                return true;
            }
            
            record.group.alpha = 0;
            ModMain.ModMono.EnqueueRefreshAvatar(record);
            return false;
        }
    }
}