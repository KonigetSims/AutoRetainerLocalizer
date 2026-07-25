using AutoRetainer.Scheduler.Tasks;
using ECommons.ExcelServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;

namespace AutoRetainer.UI.NeoUI.AdvancedEntries.DebugSection;
public unsafe class DebugRetainerTaskSupply : DebugSectionBase
{
    public override void Draw()
    {
        if(TryGetAddonByName<AtkUnitBase>("雇员任务补给", out var addon) && addon->IsReady())
        {
            for(var i = 0; i < addon->AtkValues[107].UInt; i++)
            {
                var ptr = (nint)addon->AtkValues[42 + i].Pointer;
                var id = *(uint*)ptr;
                ImGuiEx.Text($"{Svc.Data.GetExcelSheet<RetainerTask>().GetRow(id).GetVentureItem().Value.GetName()}");
            }
        }
        ref var vid = ref Ref<int>.Get("DTRSID");
        ImGui.InputInt("vid", ref vid);
        if(ImGui.Button("打开分配探险窗口")) DuoLog.Information($"{TaskAssignHuntingVenture.OpenAssignVentureWindow((uint)vid)}");
    }
}
