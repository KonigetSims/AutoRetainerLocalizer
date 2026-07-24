using ECommons.Configuration;
using ECommons.Reflection;

namespace AutoRetainer.UI.NeoUI.AdvancedEntries;
public class ExpertTab : NeoUIEntry
{
    public override string Path => "进阶设置/专家设置";

    public override NuiBuilder Builder { get; init; } = new NuiBuilder()
        .Section("行为设置")
        .EnumComboFullWidth(null, "访问雇员铃铛时若无可用探险任务的动作：", () => ref C.OpenBellBehaviorNoVentures)
        .EnumComboFullWidth(null, "访问雇员铃铛时若有可用探险任务的动作：", () => ref C.OpenBellBehaviorWithVentures)
        .EnumComboFullWidth(null, "访问铃铛后任务完成的行为：", () => ref C.TaskCompletedBehaviorAccess)
        .EnumComboFullWidth(null, "手动启用后任务完成的行为：", () => ref C.TaskCompletedBehaviorManual)
        .EnumComboFullWidth(null, "插件运行期间任务完成的行为：", () => ref C.TaskCompletedBehaviorAuto)
        .TextWrapped(ImGuiColors.DalamudGrey, "多角色模式运行期间，上述3个设置中的\"关闭雇员列表并禁用插件\"选项将被强制启用。")
        .Checkbox("如果 5 分钟内有雇员将完成探险，则停留在雇员菜单中", () => ref C.Stay5, "此选项在多角色模式运行期间强制启用。")
        .Checkbox($"关闭雇员列表时自动禁用插件", () => ref C.AutoDisable, "仅在你手动退出菜单时生效；否则将应用上方的设置。")
        .Checkbox($"不显示插件状态图标", () => ref C.HideOverlayIcons)
        .Checkbox($"显示多角色模式类型选择器", () => ref C.DisplayMMType)
        .Checkbox($"在部队工坊中显示远航探索", () => ref C.ShowDeployables)
        .Checkbox("启用应急恢复模块", () => ref C.EnableBailout)
        .InputInt(150f, "AutoRetainer尝试解除卡死前的超时时间(秒)", () => ref C.BailoutTimeout)

        .Section("设置")
        .Checkbox("Allow operating on retainers without a job", () => ref C.AllowUnemployed)
        .Widget("跳过旅馆登录动画", text =>
        {
            ImGui.SetNextItemWidth(200);
            if(ImGuiEx.EnumCombo(text, ref C.CutsceneSkipMode))
            {
                S.InnCutsceneSkip.RefreshAccordingToConfig();
            }
            ImGuiEx.HelpMarker("跳过登录动画可被服务器检测到，会增加被封禁几率", EColor.RedBright, FontAwesomeIcon.ExclamationTriangle.ToIconString());
        })
        .Checkbox($"禁用排序和折叠/展开功能", () => ref C.NoCurrentCharaOnTop)
        .Checkbox($"在插件UI栏显示多角色模式复选框", () => ref C.MultiModeUIBar)
        .SliderIntAsFloat(100f, "Retainer menu delay, seconds", () => ref C.RetainerMenuDelay.ValidateRange(0, 2000), 0, 2000)
        .Checkbox($"允许探险计时器显示负值", () => ref C.TimerAllowNegative)
        .Checkbox($"不检查探险计划错误", () => ref C.NoErrorCheckPlanner2)
        .Checkbox("启用手动重新登录后的角色后处理", () => ref C.AllowManualPostprocess, "当 AutoRetainer 锁定在后处理状态时，允许手动调用指令。")
        .Widget("市场冷却时间覆盖层", (x) =>
        {
            if(ImGui.Checkbox(x, ref C.MarketCooldownOverlay))
            {
                if(C.MarketCooldownOverlay)
                {
                    P.Memory.OnReceiveMarketPricePacketHook?.Enable();
                }
                else
                {
                    P.Memory.OnReceiveMarketPricePacketHook?.Disable();
                }
            }
        })

        .Section("插件集成")
        .Checkbox($"Artisan 集成功能", () => ref C.ArtisanIntegration, "当探险任务准备好领取且附近有雇员铃铛时，自动启用 AutoRetainer 并暂停 Artisan 的操作。当雇员任务处理完毕后，Artisan 将重新启用并恢复之前的动作")

        .Section("服务器时间")
        .Checkbox("使用服务器时间而非本地时间", () => ref C.UseServerTime)

        .Section("工具")
        .Widget("清理幽灵雇员", (x) =>
        {
            if(ImGui.Button(x))
            {
                var i = 0;
                foreach(var d in C.OfflineData)
                {
                    i += d.RetainerData.RemoveAll(x => x.Name == "");
                }
                DuoLog.Information($"已清理 {i} 个项目");
            }
        })

        .Section("导入/导出")
        .Widget(() =>
        {
            if(ImGui.Button("导出（不含角色数据）"))
            {
                var clone = C.JSONClone();
                clone.OfflineData = null;
                clone.AdditionalData = null;
                clone.FCData = null;
                clone.SelectedRetainers = null;
                clone.Blacklist = null;
                clone.AutoLogin = "";
                Copy(EzConfig.DefaultSerializationFactory.Serialize(clone, false));
            }
            if(ImGui.Button("导入并合并角色数据"))
            {
                try
                {
                    var c = EzConfig.DefaultSerializationFactory.Deserialize<Config>(Paste());
                    c.OfflineData = C.OfflineData;
                    c.AdditionalData = C.AdditionalData;
                    c.FCData = C.FCData;
                    c.SelectedRetainers = C.SelectedRetainers;
                    c.Blacklist = C.Blacklist;
                    c.AutoLogin = C.AutoLogin;
                    if(c.GetType().GetFieldPropertyUnions().Any(x => x.GetValue(c) == null)) throw new NullReferenceException();
                    EzConfig.SaveConfiguration(C, $"Backup_{DateTimeOffset.Now.ToUnixTimeMilliseconds()}.json");
                    P.SetConfig(c);
                }
                catch(Exception e)
                {
                    e.LogDuo();
                }
            }
        });
}