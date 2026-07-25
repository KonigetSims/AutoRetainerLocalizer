using AutoRetainer.Internal;
using AutoRetainer.Modules.Voyage;
using AutoRetainerAPI.Configuration;
using Dalamud.Game;
using ECommons.GameHelpers;
using ECommons.Reflection;

namespace AutoRetainer.UI.MainWindow;
public static unsafe class TroubleshootingUI
{
    private static readonly Config EmptyConfig = new();

    public static bool IsPluginInstalled(string name)
    {
        return Svc.PluginInterface.InstalledPlugins.Any(x => x.IsLoaded && (x.InternalName.EqualsIgnoreCase(name) || x.Name.EqualsIgnoreCase(name)));
    }

    public static void Draw()
    {
        ImGuiEx.TextWrapped("本选项卡将检查您的配置是否有常见问题，您可以在联系技术支持前自行解决这些问题。");

        if(!Player.Available)
        {
            ImGuiEx.TextWrapped($"未登录时无法进行故障排除。");
            return;
        }

        if(C.CutsceneSkipMode != AutoRetainerAPI.Configuration.CutsceneSkipMode.Never)
        {
            Info($"旅馆过场动画跳过模块设置为 {C.CutsceneSkipMode}。AutoRetainer 将跳过旅馆过场动画。");
        }

        if(Data == null)
        {
            Error($"找不到当前角色的数据。请打开召唤铃、探险队（派遣）面板或重新登录以生成数据。");
        }

        if(C.IgnoreGCRankCheck)
        {
            Error("已启用忽略军衔检查。请禁用以使插件正常运行。(/ays set IgnoreGCRankCheck false)");
        }

        if(!Svc.ClientState.ClientLanguage.EqualsAny(ClientLanguage.Japanese, ClientLanguage.German, ClientLanguage.French, ClientLanguage.English))
        {
            Error($"检测到非国际服客户端。AutoRetainer未在其他最终幻想14客户端上进行测试。部分或全部功能可能无法正常运作。此外，请注意，ottercorp 的中国 Dalamud 分支会在未经您同意的情况下收集有关您的电脑、角色、所用插件和 Dalamud 配置的遥测数据，并且您无法选择退出。");
        }

        if(C.DontLogout)
        {
            Error("已启用DontLogout调试选项");
        }

        if(C.FullAutoGCDelivery) 
        {
            int maxRetainersWhenGcDelivery = 0;
            var warnSub = false;
            foreach(var x in C.OfflineData)
            {
                if(x.Enabled && x.GCDeliveryType != GCDeliveryType.Disabled)
                {
                    maxRetainersWhenGcDelivery = Math.Max(maxRetainersWhenGcDelivery, x.GetEnabledRetainers(false).Length);
                }
                if(x.WorkshopEnabled && x.GetEnabledVesselsData(VoyageType.Submersible).Count > 0)
                {
                    warnSub = true;
                }
            }
            if(warnSub && C.FullAutoGCDeliveryInventory < 50)
            {
                Warning($"在多角色模式下，专家交付的空闲背包栏位触发值设置为 {C.FullAutoGCDeliveryInventory}，而潜艇模块已启用。建议至少设置为 50 以避免背包溢出问题。");
            }
            if(C.FullAutoGCDeliveryInventory < maxRetainersWhenGcDelivery * 5)
            {
                Warning($"您的一些多角色模式启用的角色有 {maxRetainersWhenGcDelivery} 个雇员已启用，而多角色模式专家交付的空闲背包栏位触发值设置为 {C.FullAutoGCDeliveryInventory}。强烈建议您将其设置为至少 {C.FullAutoGCDeliveryInventory * maxRetainersWhenGcDelivery}（每个雇员 5 个栏位）。");
            }
        }

        foreach(var x in C.OfflineData)
        {
            if(x.WorkshopEnabled)
            {
                var a = x.OfflineSubmarineData.Select(x => x.Name);
                if(a.Count() > a.Distinct().Count())
                {
                    Error($"角色 {Censor.Character(x.Name, x.World)} 的潜水艇名称存在重复。潜水艇名称必须是唯一的。");
                }
            }
        }

        if((C.GlobalTeleportOptions.Enabled || C.OfflineData.Any(x => x.TeleportOptionsOverride.Enabled == true)) && !Svc.PluginInterface.InstalledPlugins.Any(x => x.InternalName == "Lifestream" && x.IsLoaded))
        {
            Error("已启用传送功能但未安装或未加载 Lifestream 插件。在此配置下 AutoRetainer 无法运作。请禁用传送功能或安装 Lifestream 插件。");
        }

        foreach(var x in C.SubmarineUnlockPlans)
        {
            if(x.EnforcePlan)
            {
                Info($"潜水艇解锁计划 {x.Name.NullWhenEmpty() ?? x.GUID} 设置为强制执行模式，如有需要解锁的内容，将覆盖所有潜水艇设置。");
            }
        }

        foreach(var x in C.SubmarineUnlockPlans)
        {
            if(x.EnforceDSSSinglePoint)
            {
                Info($"潜水艇解锁计划 {x.Name.NullWhenEmpty() ?? x.GUID} 设置为在深海站点单点部署，并将忽略手动设置的解锁行为。");
            }
        }

        try
        {
            if(DalamudReflector.IsOnStaging())
            {
                Error($"检测到非正式版Dalamud分支。这可能导致问题。请通过输入/xlbranch打开分支切换器，切换到 \"release\" 分支并重新启动游戏");
            }
        }
        catch(Exception e)
        {
        }

        if(Player.Available)
        {
            if(Player.CurrentWorld != Player.HomeWorld)
            {
                Error("您正在访问其他服务器。必须返回原始服务器后，AutoRetainer才能继续处理此角色。");
            }
            if(C.Blacklist.Any(x => x.CID == Player.CID))
            {
                Error("当前角色已完全排除在AutoRetainer处理之外。请前往设置→排除项进行变更。");
            }
            if(Data?.ExcludeRetainer == true)
            {
                Error("当前角色已被排除在雇员列表外。请前往设置→排除项进行变更。");
            }
            if(Data?.ExcludeWorkshop == true)
            {
                Error("当前角色已被排除在远航探索列表外。请前往设置→排除项进行变更。");
            }
        }

        {
            var list = C.OfflineData.Where(x => x.GetAreTeleportSettingsOverriden());
            if(list.Any())
            {
                Info("部分角色的传送选项已自定义。鼠标悬停查看列表。", list.Select(x => $"{x.Name}@{x.World}").Print("\n"));
            }
        }

        if(C.NoTeleportHetWhenNextToBell)
        {
            Warning("当角色靠近召唤铃时，传送或进入房屋/公寓的功能已被禁用。请注意房屋拆除计时器。");
        }



        if(C.AllowSimpleTeleport)
        {
            Warning("已启用简单传送选项。此选项不如在Lifestream中登记房屋可靠。如遇到传送问题，请考虑禁用此选项并在Lifestream中登记您的房屋。");
        }

        if(!C.EnableEntrustManager && C.AdditionalData.Any(x => x.Value.EntrustPlan != Guid.Empty))
        {
            Warning($"托管管理器已全局禁用，但部分雇员已分配托管计划。托管计划将仅在手动操作时处理。");
        }

        if(C.ExtraDebug)
        {
            Info("已启用额外日志记录选项。这将导致日志大量输出，请仅在收集调试信息时使用。");
        }

        if(C.UnsyncCompensation > -5)
        {
            Warning("时间不同步补偿值设置过高(>-5)，可能导致问题。");
        }

        if(UIUtils.GetFPSFromMSPT(C.TargetMSPTIdle) < 10)
        {
            Warning("空闲时帧率设置过低(<10)，可能导致问题。");
        }

        if(UIUtils.GetFPSFromMSPT(C.TargetMSPTRunning) < 20)
        {
            Warning("运行时的帧率设置过低(<20)，可能导致问题。");
        }

        if(Data?.GetIMSettings().AllowSellFromArmory == true)
        {
            Info("已启用允许从装备库出售物品选项。请确保将您的零式装备和绝境武器加入保护列表。");
        }

        {
            var list = C.OfflineData.Where(x => !x.ExcludeRetainer && !x.Enabled && x.RetainerData.Count > 0);
            if(list.Any())
            {
                Warning($"部分角色未启用雇员多角色模式，但已登记雇员。鼠标悬停查看列表。", list.Print("\n"));
            }
        }
        {
            var list = C.OfflineData.Where(x => !x.ExcludeRetainer && x.Enabled && x.RetainerData.Count > 0 && C.SelectedRetainers.TryGetValue(x.CID, out var rd) && !x.RetainerData.All(r => rd.Contains(r.Name)));
            if(list.Any())
            {
                Warning($"部分角色未启用所有雇员进行处理。鼠标悬停查看列表。", list.Print("\n"));
            }
        }
        {
            var list = C.OfflineData.Where(x => !x.ExcludeWorkshop && !x.WorkshopEnabled && (x.OfflineSubmarineData.Count + x.OfflineAirshipData.Count) > 0);
            if(list.Any())
            {
                Warning($"部分角色未启用远航探索多角色模式，但已登记远航探索。鼠标悬停查看列表。", list.Print("\n"));
            }
        }

        {
            var list = C.OfflineData.Where(x => !x.ExcludeWorkshop && x.WorkshopEnabled && x.GetEnabledVesselsData(Internal.VoyageType.Airship).Count + x.GetEnabledVesselsData(Internal.VoyageType.Submersible).Count < Math.Min(x.OfflineAirshipData.Count + x.OfflineSubmarineData.Count, 4));
            if(list.Any())
            {
                Warning($"部分角色未启用所有远航探索进行处理。鼠标悬停查看列表。", list.Print("\n"));
            }
        }

        if(C.MultiModeType != AutoRetainerAPI.Configuration.MultiModeType.Everything)
        {
            Warning($"您的多角色模式类型设置为 {C.MultiModeType}；这将限制AutoRetainer执行的功能。");
        }

        if(C.OfflineData.Any(x => x.MultiWaitForAllDeployables))
        {
            Info("部分角色已启用了\"等待所有待处理潜艇\"选项。这代表对于这些角色，AutoRetainer 会等到所有潜艇回归后才开始处理。将光标悬停在此处可查看启用了此选项的角色列表。", C.OfflineData.Where(x => x.MultiWaitForAllDeployables).Select(x => $"{x.Name}@{x.World}").Print("\n"));
        }

        if(C.MultiModeWorkshopConfiguration.MultiWaitForAll)
        {
            Info("全局选项\"等待探险完成\"已启用。这代表对于所有角色，AutoRetainer 都会等到所有雇员回归后才处理，即使该角色的独立选项已关闭也是如此。");
        }

        if(C.MultiModeWorkshopConfiguration.WaitForAllLoggedIn)
        {
            Info("潜艇已启用「即使已登录也等待」选项。这代表即使你已在线，AutoRetainer 仍会等到该角色的所有潜艇任务完成后才进行处理。");
        }

        if(C.DisableRetainerVesselReturn > 0)
        {
            if(C.DisableRetainerVesselReturn > 10)
            {
                Warning("\"雇员探险处理截止时间\"被设置为异常高值。当雇员即将可用时，你可能会在重新派遣雇员时遇到明显延迟。");
            }
            else
            {
                Info("\"雇员探险处理截止时间\"已启用。当雇员即将可用时，你可能会在重新派遣雇员时遇到明显延迟。");
            }
        }

        if(C.MultiModeRetainerConfiguration.MultiWaitForAll)
        {
            Info("\"等待探险完成\"选项已启用。这代表 AutoRetainer 会等到该角色的所有雇员探险都完成后，才会登录并处理。");
        }

        if(C.MultiModeRetainerConfiguration.WaitForAllLoggedIn)
        {
            Info("雇员已启用\"即使已登录也等待\"选项。这代表即使你已在线，AutoRetainer 仍会等到该角色的所有雇员探险完成后才进行处理。");
        }

        {
            var manualList = new List<string>();
            var deletedList = new List<string>();
            foreach(var x in C.OfflineData)
            {
                foreach(var ret in x.RetainerData)
                {
                    var planId = Utils.GetAdditionalData(x.CID, ret.Name).EntrustPlan;
                    var plan = C.EntrustPlans.FirstOrDefault(s => s.Guid == planId);
                    if(plan != null && plan.ManualPlan) manualList.Add($"{Censor.Character(x.Name)} - {Censor.Retainer(ret.Name)}");
                    if(plan == null && planId != Guid.Empty) deletedList.Add($"{Censor.Character(x.Name)} - {Censor.Retainer(ret.Name)}");
                }
            }
            if(manualList.Count > 0)
            {
                Info("你的一些雇员设置了手动存放计划。这些计划在重新派遣雇员后不会自动执行，只能通过点击覆盖界面上的按钮来手动触发。将光标悬停以查看名单。", manualList.Print("\n"));
            }
            if(deletedList.Count > 0)
            {
                Warning("你的一些雇员存放计划先前已被删除。这些雇员将不会存放任何物品。将光标悬停以查看名单。", deletedList.Print("\n"));
            }
        }

        if(C.No2ndInstanceNotify)
        {
            Info("你启用了\"不针对从相同目录运行的第二个游戏实例进行警告\"，这会让 AutoRetainer 在检测到使用相同 Dalamud 目录的第二个游戏窗口时，自动跳过该窗口的加载。");
        }

        if(Svc.PluginInterface.InstalledPlugins.Any(x => x.InternalName == "SimpleTweaksPlugin" && x.IsLoaded))
        {
            Info("检测到 Simple Tweaks 插件。任何与雇员或潜水艇相关的微调都可能对 AutoRetainer 的功能造成负面影响。请确保微调设置不会干扰 AutoRetainer 的运作。");
        }

        if(Svc.PluginInterface.InstalledPlugins.Any(x => x.InternalName == "PandorasBox" && x.IsLoaded))
        {
            Info("检测到 Pandora's Box 插件。在 AutoRetainer 启用时自动执行动作可能会造成负面影响。请确保当 AutoRetainer 处于活动状态时，Pandora's Box 不会自动执行任何动作。");
        }

        if(Svc.PluginInterface.InstalledPlugins.Any(x => x.InternalName == "Automaton" && x.IsLoaded))
        {
            Info("检测到 Automaton 插件。在 AutoRetainer 启用时自动执行动作或自动输入数值可能会造成负面影响。请确保在 AutoRetainer 活动期间，Automaton 不会自动执行动作。");
        }

        if(Svc.PluginInterface.InstalledPlugins.Any(x => x.InternalName == "RotationSolver" && x.IsLoaded))
        {
            Info("检测到 RotationSolver 插件。在 AutoRetainer 启用时自动执行技能可能会造成负面影响。请确保在 AutoRetainer 活动期间，RotationSolver 不会自动执行动作。");
        }

        if(Svc.PluginInterface.InstalledPlugins.Any(x => x.InternalName.StartsWith("BossMod") && x.IsLoaded))
        {
            Info("检测到 BossMod 插件。在 AutoRetainer 启用时自动执行动作可能会造成负面影响。请确保在 AutoRetainer 活动期间，BossMod 不会自动执行动作。");
        }

        ImGui.Separator();
        ImGuiEx.TextWrapped("专家设置会修改开发者预期的行为。请检查你的问题是否与错误配置的专家设置有关。");
        CheckExpertSetting("无可用探险任务时访问召唤铃的操作", nameof(C.OpenBellBehaviorNoVentures));
        CheckExpertSetting("有可用探险任务时访问召唤铃的操作", nameof(C.OpenBellBehaviorWithVentures));
        CheckExpertSetting("访问召唤铃后任务完成行为", nameof(C.TaskCompletedBehaviorAccess));
        CheckExpertSetting("手动启用后任务完成行为", nameof(C.TaskCompletedBehaviorManual));
        CheckExpertSetting("如果 5 分钟内有雇员将完成探险，则停留在雇员菜单中", nameof(C.Stay5));
        CheckExpertSetting("关闭雇员列表时自动禁用插件", nameof(C.AutoDisable));
        CheckExpertSetting("不显示插件状态图标", nameof(C.HideOverlayIcons));
        CheckExpertSetting("显示多角色模式类型选择器", nameof(C.DisplayMMType));
        CheckExpertSetting("在部队工坊中显示远航探索", nameof(C.ShowDeployables));
        CheckExpertSetting("启用应急恢复模块", nameof(C.EnableBailout));
        CheckExpertSetting("AutoRetainer尝试解除卡死前的超时时间(秒)", nameof(C.BailoutTimeout));
        CheckExpertSetting("禁用排序和折叠/展开功能", nameof(C.NoCurrentCharaOnTop));
        CheckExpertSetting("在插件UI栏显示多角色模式复选框", nameof(C.MultiModeUIBar));
        CheckExpertSetting("雇员菜单延迟(秒)", nameof(C.RetainerMenuDelay));
        CheckExpertSetting("不检查探险计划错误", nameof(C.NoErrorCheckPlanner2));
        CheckExpertSetting("启用多角色模式时，尝试进入附近房屋", nameof(C.MultiHETOnEnable));
        CheckExpertSetting("Artisan 集成功能", nameof(C.ArtisanIntegration));
        CheckExpertSetting("使用服务器时间而非本地时间", nameof(C.UseServerTime));
    }

    private static void Error(string message, string tooltip = null)
    {
        ImGui.PushFont(UiBuilder.IconFont);
        ImGuiEx.Text(EColor.RedBright, "");
        ImGui.PopFont();
        if(tooltip != null) ImGuiEx.Tooltip(tooltip);
        ImGui.SameLine();
        ImGuiEx.TextWrapped(EColor.RedBright, message);
        if(tooltip != null) ImGuiEx.Tooltip(tooltip);
    }

    private static void Warning(string message, string tooltip = null)
    {
        ImGui.PushFont(UiBuilder.IconFont);
        ImGuiEx.Text(EColor.OrangeBright, "");
        ImGui.PopFont();
        if(tooltip != null) ImGuiEx.Tooltip(tooltip);
        ImGui.SameLine();
        ImGuiEx.TextWrapped(EColor.OrangeBright, message);
        if(tooltip != null) ImGuiEx.Tooltip(tooltip);
    }

    private static void Info(string message, string tooltip = null)
    {
        ImGui.PushFont(UiBuilder.IconFont);
        ImGuiEx.Text(EColor.YellowBright, "");
        ImGui.PopFont();
        if(tooltip != null) ImGuiEx.Tooltip(tooltip);
        ImGui.SameLine();
        ImGuiEx.TextWrapped(EColor.YellowBright, message);
        if(tooltip != null) ImGuiEx.Tooltip(tooltip);
    }

    private static void CheckExpertSetting(string setting, string nameOfSetting)
    {
        var original = EmptyConfig.GetFoP(nameOfSetting);
        var current = C.GetFoP(nameOfSetting);
        if(!original.Equals(current))
        {
            Info($"专家设置 \"{setting}\" 与默认值不同", $"默认值为 \"{original}\", 当前值为 \"{current}\".");
        }
    }
}
