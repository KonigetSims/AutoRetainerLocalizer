using AutoRetainerAPI.Configuration;
using System.Collections.Frozen;

namespace AutoRetainer.UI.NeoUI.MultiModeEntries;
public class MultiModeContingency : NeoUIEntry
{
    private static readonly FrozenDictionary<WorkshopFailAction, string> WorkshopFailActionNames = new Dictionary<WorkshopFailAction, string>()
    {
        [WorkshopFailAction.StopPlugin] = "Halt all plugin operation",
        [WorkshopFailAction.ExcludeVessel] = "Exclude deployable from operation",
        [WorkshopFailAction.ExcludeChar] = "Exclude captain from multi mode rotation",
    }.ToFrozenDictionary();

    public override string Path => "多角色模式/应急设置";

    public override NuiBuilder Builder { get; init; } = new NuiBuilder()
        .Section("应急设置")
        .TextWrapped("在此配置各种常见故障状态或潜在操作错误时的紧急方案")
        .EnumComboFullWidth(null, "青磷水耗尽", () => ref C.FailureNoFuel, (x) => x != WorkshopFailAction.ExcludeVessel, WorkshopFailActionNames, "当青磷水不足以进行新航次时，执行所选的后备方案（如中止或切换角色）")
        .EnumComboFullWidth(null, "无法维修舰艇", () => ref C.FailureNoRepair, null, WorkshopFailActionNames, "魔导修理材料不足以修理潜艇时，执行所选的后备方案。")
        .EnumComboFullWidth(null, "背包空间不足", () => ref C.FailureNoInventory, (x) => x != WorkshopFailAction.ExcludeVessel, WorkshopFailActionNames, "当身上背包空间不足以接收航行奖励时，执行所选的后备方案。")
        .EnumComboFullWidth(null, "关键操作失败", () => ref C.FailureGeneric, (x) => x != WorkshopFailAction.ExcludeVessel, WorkshopFailActionNames, "发生任何未知或杂项错误时，执行所选的后备方案。")
        .Widget("被 GM 关监狱", (x) =>
        {
            ImGui.BeginDisabled();
            ImGuiEx.SetNextItemFullWidth();
            if(ImGui.BeginCombo("##jailsel", "强制结束游戏")) { ImGui.EndCombo(); }
            ImGui.EndDisabled();
        }, "如果你在插件执行期间被 GM 关进小黑屋（监狱）时，执行所选的后备方案。祝你好运！");
}
