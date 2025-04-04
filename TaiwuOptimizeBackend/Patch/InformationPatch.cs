using System.Collections.Generic;
using System.Reflection;

namespace TaiwuOptimizeBackend.Patch
{
    // [HarmonyPatch(typeof(GameData.Domains.Information.Collection.SecretInformationCollection),"RemoveSenselessSecretInformation")]
    // public class RemoveSenselessSecretInformation {
    public class InformationPatch
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            foreach (MethodInfo i in typeof(GameData.Domains.Information.Collection.SecretInformationCollection)
                         .GetMethods())
            {
                ModMain.Log("修改类别" + i.ToString() + "的MoveNext方法");
                // if(AccessTools.EnumeratorMoveNext(i)!=null){yield return AccessTools.EnumeratorMoveNext(i);}else{logger("error:i="+i.ToString()+", have no move next");}
                if (i.Name.Length > 3 && i.Name[0] == 'A' && i.Name[1] == 'd' && i.Name[2] == 'd')
                {
                    yield return i;
                }
            }

            yield return typeof(GameData.Domains.Information.InformationDomain).GetMethod(
                "AddSecretInformationMetaData");
        }

        static bool Prefix(ref int __result)
        {
            __result = 0;
            return false;
        }
        // public void RemoveSenselessSecretInformation(DataContext context)
    }
}