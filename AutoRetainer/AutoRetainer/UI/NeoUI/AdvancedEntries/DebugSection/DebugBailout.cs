namespace AutoRetainer.UI.NeoUI.AdvancedEntries.DebugSection;

internal class DebugBailout : DebugSectionBase
{
    public override void Draw()
    {
        ImGui.Checkbox(nameof(BailoutManager.SimulateStuckOnQuit), ref BailoutManager.SimulateStuckOnQuit);
        ImGui.Checkbox(nameof(BailoutManager.SimulateStuckOnVoyagePanel), ref BailoutManager.SimulateStuckOnVoyagePanel);
        ImGuiEx.Text($"无选择字符串: {Environment.TickCount64 - BailoutManager.NoSelectString}");
        ImGuiEx.Text($"大厅卡死: {Environment.TickCount64 - BailoutManager.CharaSelectStuck}");
    }
}
