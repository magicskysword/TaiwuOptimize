// dotnet $DOTNET_CSC_DLL -nologo -t:library -r:"../../../../compatdata/838350/pfx/drive_c/Program Files/dotnet/shared/Microsoft.NETCore.App/5.0.17/System.dll" -r:"../../../../compatdata/838350/pfx/drive_c/Program Files/dotnet/shared/Microsoft.NETCore.App/5.0.17/System.Collections.dll" -r:"../../The Scroll of Taiwu_Data/Managed/0Harmony.dll" -r:"../../../../compatdata/838350/pfx/drive_c/Program Files/dotnet/shared/Microsoft.NETCore.App/5.0.17/mscorlib.dll" -r:"../../Backend/GameData.dll" -r:"../../Backend/Redzen.dll" -r:"../../The Scroll of Taiwu_Data/Managed/TaiwuModdingLib.dll" -r:"../../../../compatdata/838350/pfx/drive_c/Program Files/dotnet/shared/Microsoft.NETCore.App/5.0.17/System.Private.CoreLib.dll" -r:"../../../../compatdata/838350/pfx/drive_c/Program Files/dotnet/shared/Microsoft.NETCore.App/5.0.17/System.Runtime.dll" -r:"../../../../compatdata/838350/pfx/drive_c/Program Files/dotnet/shared/Microsoft.NETCore.App/5.0.17/System.Threading.Tasks.Parallel.dll" -r:"../../../../compatdata/838350/pfx/drive_c/Program Files/dotnet/shared/Microsoft.NETCore.App/5.0.17/System.Collections.Concurrent.dll" -unsafe -optimize -deterministic Backend.cs -out:Backend.dll
// -r:"../../../../compatdata/838350/pfx/drive_c/Program Files/dotnet/shared/Microsoft.NETCore.App/5.0.17/System.IO.FileSystem.dll"

//! 编译方法，目前来说只能是自己改上面那行字，把缺的东西都补齐，之后送命令提示符或者用python脚本编译。
//! windows就是一坨shit……连通配符都不支持……不然我至少能写出类似 dotnet C:\Program Files\dotnet\sdk\*\Roslyn\bincore\csc.dll 这样的语法

// -r:"../../The Scroll of Taiwu_Data/Managed/Mono.Cecil.dll" -r:"../../The Scroll of Taiwu_Data/Managed/System.Core.dll"   -r:"../../The Scroll of Taiwu_Data/Managed/System.Composition.AttributedModel.dll" -r:"../../../../compatdata/838350/pfx/drive_c/Program Files/dotnet/shared/Microsoft.NETCore.App/5.0.17/System.Runtime.dll"
/**
 *  Everyone's Unity Game Plugin
 *  Copyright (C) 2022 Neutron3529
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as
 *  published by the Free Software Foundation, either version 3 of the
 *  License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Affero General Public License for more details.
 *
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */
//! 这是一个自带文档注释的Mod，虽然代码很shit，但里面有不少注释，或许会对之后准备玩过月逻辑的人有帮助。
//! 所有文档注释会用//!开头，请注意搜索
//! 首先是前后端，前端对后端的调用会先被塞进 ProcessMethodCall，之后分发给对应domain的CallMethod，由CallMethod进行进一步的分发。借助这一点，我们可以快速找到前端调用究竟是在使用哪个函数。
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using HarmonyLib;

namespace Optim;
[TaiwuModdingLib.Core.Plugin.PluginConfig("Optim","Neutron3529","0.1.0")]
public partial class Optim : TaiwuModdingLib.Core.Plugin.TaiwuRemakeHarmonyPlugin {
    public static int Remove_Gap=12;
    private bool enable(string key){
        bool enabled=false;
        //if(key=="N02")return GameData.Domains.DomainManager.Mod.GetSetting(this.ModIdStr, key, ref Remove_Gap) && Remove_Gap>1;
        return GameData.Domains.DomainManager.Mod.GetSetting(this.ModIdStr, key, ref enabled) && enabled;
    }
    public static void logger(string s){
        GameData.Utilities.AdaptableLog.Info(s);
    }
    public override void OnModSettingUpdate(){
        this.HarmonyInstance.UnpatchSelf();
        if(typeof(GameData.Domains.Organization.OrganizationDomain).GetField("ParallelUpdateOrganizationMembers")==null && enable("N01"))this.HarmonyInstance.PatchAll(typeof(UpdateOrganizationMembers));
        if(typeof(GameData.Domains.Information.Collection.SecretInformationCollection).GetField("FastEnough")==null && enable("N02"))this.HarmonyInstance.PatchAll(typeof(FxxkInformation));
    }
    public override void Initialize() {
    }

    [HarmonyPatch(typeof(GameData.Domains.Organization.OrganizationDomain),"UpdateOrganizationMembers")]
    public class UpdateOrganizationMembers {
        public static MethodInfo UpdateFactions=typeof(GameData.Domains.Organization.OrganizationDomain).GetMethod("UpdateFactions",(BindingFlags)(-1));
        public static MethodInfo UpdateAllMentorsAndMenteesInSect=typeof(GameData.Domains.Organization.OrganizationDomain).GetMethod("UpdateAllMentorsAndMenteesInSect",(BindingFlags)(-1));
        public static FieldInfo _membersSortedByCombatPower=typeof(GameData.Domains.Organization.Settlement).GetField("_membersSortedByCombatPower",(BindingFlags)(-1));
        public static FieldInfo Members=typeof(GameData.Domains.Organization.Settlement).GetField("Members",(BindingFlags)(-1));
        public static FieldInfo _combatPower=typeof(GameData.Domains.Character.Character).GetField("_combatPower",(BindingFlags)(-1));
        public static bool Prefix(GameData.Common.DataContext context, Dictionary<short, GameData.Domains.Organization.Settlement> ____settlements, GameData.Domains.Organization.OrganizationDomain __instance) {
            int currDate = GameData.Domains.DomainManager.World.GetCurrDate();
            Dictionary<int, System.ValueTuple<GameData.Domains.Character.Character, short>> baseInfluencePowers = new Dictionary<int, System.ValueTuple<GameData.Domains.Character.Character, short>>();
            HashSet<int> relatedCharIds = new HashSet<int>();
            foreach (GameData.Domains.Organization.Settlement settlement in ____settlements.Values) {
                sbyte orgTemplateId = settlement.GetOrgTemplateId();
                settlement.UpdateMemberGrades(context);
                short influencePowerUpdateInterval = Config.Organization.Instance[orgTemplateId].InfluencePowerUpdateInterval;
                int influencePowerUpdateDate = settlement.GetInfluencePowerUpdateDate();
                if (influencePowerUpdateInterval > 0 && currDate >= influencePowerUpdateDate) {
                    settlement.UpdateInfluencePowers(context, baseInfluencePowers, relatedCharIds);
                    settlement.SetInfluencePowerUpdateDate(currDate + (int)influencePowerUpdateInterval, context);
                    if (GameData.Domains.Organization.OrganizationDomain.IsSect(settlement.GetOrgTemplateId())) {
                        UpdateFactions.Invoke(__instance, new object[]{context, settlement});
                    }
                }
            }
            System.Threading.Tasks.Parallel.ForEach(____settlements.Values,(i)=>{
                //var id=(short)typeof(GameData.Domains.Organization.Settlement).GetField("Id",(BindingFlags)(-1)).GetValue(i);
                //var sw=System.Diagnostics.Stopwatch.StartNew();
                //logger("swstart:"+id);
                {   //这里实现了i.SortMembersByCombatPower();
                    var lst=((SortedList<long,int>)(_membersSortedByCombatPower.GetValue(i)));
                    var member=((GameData.Domains.Organization.OrgMemberCollection)Members.GetValue(i));
                    lst.Clear();
                    for (sbyte grade = 0; grade < 9; grade += 1)
                    {
                        HashSet<int> members = member.GetMembers(grade);
                        foreach (int memberCharId in members)
                        {
                            GameData.Domains.Character.Character character = GameData.Domains.DomainManager.Character.GetElement_Objects(memberCharId);
                            int combatPower = (int)_combatPower.GetValue(character); // TODO: 需要保证在前面的线程中并行更新每个人的战斗力，但就算不更新，误差也不会太大。
                            long key = ((long)combatPower << 32) + (long)memberCharId;
                            lst.Add(key, memberCharId);
                        }
                    }
                }
                //logger("swend:"+id+" cost"+sw.Elapsed.TotalMilliseconds);
            });
            foreach (GameData.Domains.Organization.Settlement settlement in ____settlements.Values) {
                GameData.Domains.Organization.Sect sect = settlement as GameData.Domains.Organization.Sect;
                if (sect != null)
                {
                    sect.UpdateMartialArtTournamentPreparations();
                    bool flag4 = currDate % 3 == 0;
                    if (flag4)
                    {
                        sect.UpdateApprovalOfTaiwu(context);
                    }
                    UpdateAllMentorsAndMenteesInSect.Invoke(__instance, new object[]{context, sect});
                }
            }
            return false;
        }
    }



    // [HarmonyPatch(typeof(GameData.Domains.Information.Collection.SecretInformationCollection),"RemoveSenselessSecretInformation")]
    // public class RemoveSenselessSecretInformation {
    public class FxxkInformation {
        static IEnumerable<MethodBase> TargetMethods(){
            foreach(MethodInfo i in typeof(GameData.Domains.Information.Collection.SecretInformationCollection).GetMethods()){
                if(i==null){logger("FATAL:NULL i");}else{
                    logger("修改类别"+i.ToString()+"的MoveNext方法");
                    // if(AccessTools.EnumeratorMoveNext(i)!=null){yield return AccessTools.EnumeratorMoveNext(i);}else{logger("error:i="+i.ToString()+", have no move next");}
                    if(i.Name.Length>3 && i.Name[0]=='A' && i.Name[1]=='d' && i.Name[2]=='d' ) {
                        yield return i;
                    }
                }
            }
            yield return typeof(GameData.Domains.Information.InformationDomain).GetMethod("AddSecretInformationMetaData");
        }
        static bool Prefix(ref int __result){
            __result=0;
            return false;
        }
        // public void RemoveSenselessSecretInformation(DataContext context)
    }
}
