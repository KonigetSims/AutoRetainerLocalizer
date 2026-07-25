using Dalamud.Interface.Components;

namespace AutoRetainer.UI.NeoUI.AdvancedEntries.DebugSection;

internal class SuperSecret : DebugSectionBase
{
    public override void Draw()
    {
        ImGuiEx.TextWrapped(ImGuiColors.ParsedOrange, "这里可能会发生任何状况");
        ImGui.Checkbox("旧版召唤铃感应", ref C.OldRetainerSense);
        ImGuiComponents.HelpMarker("检测并使用玩家有效距离内最近的召唤铃");
        ImGuiEx.TextWrapped(ImGuiColors.DalamudGrey, "在多角色模式执行期间，强制启用召唤铃感应");
        ImGui.Separator();
        ImGui.Checkbox($"不安全选项保护", ref C.UnsafeProtection);
        ImGui.SameLine();
        if(ImGui.Button($"写入注册表"))
        {
            Safety.Set(C.UnsafeProtection);
        }
        var g = Safety.Get();
        ImGuiEx.Text(g ? ImGuiColors.ParsedGreen : ImGuiColors.DalamudRed, $"安全标记: {(g ? "Present" : "Absent")}");
        ImGui.Separator();
        ImGuiEx.Checkbox("在多角色模式下忽略军衔检查", ref C.IgnoreGCRankCheck);
    }
}
