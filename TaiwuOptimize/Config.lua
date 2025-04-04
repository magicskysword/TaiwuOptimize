return {
    Title = "太吾优化",
    Author = "剑圣(skyswordkill)",
    Description = "原名：流畅滚动列表\
改善游戏内卡顿与部分操作逻辑。\
所有优化项都可以选择性打开或关闭。\
\
优化项：\
* 人物头像加载逻辑，从瞬间加载改为了分时加载。若对头像淡入效果不适可以在设置中改为立即显示。\
* 对话界面弹出加快，修改弹出样式：向下弹出->淡入\
* 为战斗界面加回了攻击范围显示\
* 【需手动开启】加快战斗结算的动画时间\
* 优化奇遇的行走方式，不再是走一格停一格\
* 【需手动开启】加快奇遇的移动速度\
* 【需手动开启】加快奇遇的格子展开速度\
* 优化大地图移动方式\
* 【需手动开启】加快大地图移动速度\
\
已删除：\
* 【官方已优化】游戏滚动列表逻辑\
* 【官方已优化】势力界面加载\
* 【官方已优化】加快了过月速度：对过月组织成员排序进行优化（感谢@Neutron3529）\
* 【存在Bug待更新】战斗开始前的等待时间提前到先手提示出现\
* 【存在Bug待更新】加快了过月速度：关闭过月时的秘闻系统更新（感谢@Neutron3529）\
\
官方已优化硬件鼠标Mod，目前不再需要硬件鼠标Mod。\
\
讨论Mod、反馈Bug或催更欢迎加入QQ群：689609241\
",
    Cover = "Cover.png",
    FileId = 2874712856,
    GameVersion = "1.0.0.0",
    Source = 0,
    FrontendPlugins =
    {
        [1] = "TaiwuOptimize.dll"
    },
    TagList = {
        [1] = "Optimizations",
        [2] = "Compatible Mods",
    },
    BackendPlugins =
    {
        [1] = "TaiwuOptimizeBackend.dll"
    },
    DefaultSettings =
    {
        {
            DisplayName = "立绘延迟加载",
            Description = "使立绘加载变为延迟加载，缓解加载大量立绘时的卡顿现象。",
            Key = "Bool_EnableAvatarDelayRefresh",
            DefaultValue = true,
            SettingType = "Toggle"
        },
        {
            DisplayName = "立绘加载淡入时间",
            Description = "<color=#FF0000>[需开启立绘延迟加载]</color>在立绘加载后，淡入显示的时间。对立绘淡入感觉不适可以设置为[立即显示]",
            Key = "Int_AvatarFadeInTimeTypeR",
            DefaultValue = 0,
            SettingType = "Dropdown",
            Options = {
                "立即显示",
                "0.3秒",
                "0.5秒",
                "1秒",
            }
        },
        {
            DisplayName = "使用淡入对话框",
            Description = "将对话框动画改为淡入",
            Key = "Bool_EnableEventWindowFade",
            DefaultValue = true,
            SettingType = "Toggle"
        },
        {
            DisplayName = "对话框弹出动画时间",
            Description = "人物对话框的弹出动画时间。",
            Key = "Int_EventWindowFadeTimeType",
            DefaultValue = 0,
            SettingType = "Dropdown",
            Options = {
                "0.3秒",
                "0.5秒",
                "1秒",
                "立即显示",
            }
        },
        {
            DisplayName = "启用攻击范围",
            Description = "战斗时，在角色下方显示攻击范围",
            Key = "Bool_EnableCombatRange",
            DefaultValue = true,
            SettingType = "Toggle"
        },
        {
            DisplayName = "战斗结算倍速",
            Description = "战斗结算界面的动画倍速。",
            Key = "Int_CombatResultAnimScaleType",
            DefaultValue = 0,
            SettingType = "Dropdown",
            Options = {
                "1x",
                "1.25x",
                "1.5x",
                "1.75x",
                "2x（推荐）",
                "3x",
                "4x",
                "5x",
            }
        },
        {
            DisplayName = "奇遇移动优化",
            Description = "使奇遇移动更加流畅（不用走一下等一下）",
            Key = "Bool_EnableQuickAdventure",
            DefaultValue = true,
            SettingType = "Toggle"
        },
        {
            DisplayName = "奇遇镜头跟随",
            Description = "使奇遇移动过程中镜头跟随人物",
            Key = "Bool_EnableAdventureCameraFollow",
            DefaultValue = true,
            SettingType = "Toggle"
        },
        {
            DisplayName = "奇遇移动倍速",
            Description = "奇遇界面的移动速度倍速。",
            Key = "Int_AdventureMoveSpeedScaleType",
            DefaultValue = 0,
            SettingType = "Dropdown",
            Options = {
                "1x",
                "1.25x",
                "1.5x（推荐）",
                "1.75x",
                "2x",
                "3x",
                "4x",
                "5x",
            }
        },
        {
            DisplayName = "奇遇展开倍速",
            Description = "奇遇界面的奇遇格子展开速度倍速。",
            Key = "Int_AdventureUnfoldAnimationTimeScaleType",
            DefaultValue = 0,
            SettingType = "Dropdown",
            Options = {
                "1x",
                "1.25x",
                "1.5x（推荐）",
                "1.75x",
                "2x",
                "3x",
                "4x",
                "5x",
            }
        },
        {
            DisplayName = "大地图移动优化",
            Description = "优化大地图移动，使移动更加流畅。",
            Key = "Bool_EnableQuickWorldMapMove",
            DefaultValue = true,
            SettingType = "Toggle"
        },
        {
            DisplayName = "大地图线性移动",
            Description = "<color=#FF0000>[需开启大地图移动优化]</color>使大地图移动变为线性移动动画，在连续移动时更流畅。",
            Key = "Bool_EnableLinerWorldMapMove",
            DefaultValue = true,
            SettingType = "Toggle"
        },
        {
            DisplayName = "大地图移动倍速",
            Description = "<color=#FF0000>[需开启大地图移动优化]</color>大地图的移动速度倍速。",
            Key = "Int_WorldMapMoveSpeedScaleType",
            DefaultValue = 0,
            SettingType = "Dropdown",
            Options = {
                "1x",
                "1.25x（推荐）",
                "1.5x",
                "1.75x",
                "2x",
                "3x",
                "4x",
                "5x",
            }
        },
--         {
--             DisplayName = "关闭过月秘闻",
--             Description = "关闭过月产生的秘闻，加快过月速度。",
--             Key = "Bool_DisableSecretInformationSystem",
--             DefaultValue = false,
--             SettingType = "Toggle"
--         },
    }
}