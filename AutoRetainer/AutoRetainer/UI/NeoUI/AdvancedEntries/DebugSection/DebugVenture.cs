using AutoRetainer.Scheduler.Handlers;
using AutoRetainer.Scheduler.Tasks;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace AutoRetainer.UI.NeoUI.AdvancedEntries.DebugSection;

internal unsafe class DebugVenture : DebugSectionBase
{
    internal int VentureID = 0;
    internal string VentureName = "";
    public override void Draw()
    {
        {
            var agent = AgentModule.Instance()->GetAgentByInternalId((AgentId)140);
            if(agent != null && agent->IsAgentActive())
            {
                ImGuiEx.TextCopy($"{(nint)agent:X16}");
                ImGuiEx.Text($"{*(ushort*)((uint)agent + 456)}");
            }
        }
        if(TryGetAddonByName<AddonRetainerTaskAsk>("雇员任务询问", out var addon) && IsAddonReady(&addon->AtkUnitBase))
        {
            ImGuiEx.Text($"已启用: {addon->AssignButton->IsEnabled}");
        }

        foreach(var x in C.OfflineData)
        {
            foreach(var r in x.RetainerData)
            {
                var adata = Utils.GetAdditionalData(x.CID, r.Name);
                ImGuiEx.Text($"{x.Name}@{x.World} - {r.Name} 上次探险索引: {adata.VenturePlanIndex}, 下次探险: {adata.GetNextPlannedVenture()}/{VentureUtils.GetVentureName(adata.GetNextPlannedVenture())}");
            }
        }
        ImGui.InputInt("探险 ID", ref VentureID);
        ImGui.InputText("探险名称", ref VentureName, 100);
        //if (ImGui.Button("SearchVentureByName")) DuoLog.Information(RetainerHandlers.SearchVentureByName(VentureName).ToString());
        if(ImGui.Button("清除探险列表")) DuoLog.Information(RetainerHandlers.ClearTaskSupplylist().ToString());
        if(ImGui.Button("选择特定探险名称")) DuoLog.Information(RetainerHandlers.SelectSpecificVentureByName(VentureName).ToString());
        if(ImGui.Button("任务分配狩猎探险"))
        {
            TaskAssignHuntingVenture.Enqueue((uint)VentureID);
        }
        if(ImGui.Button("任务分配领域探索"))
        {
            TaskAssignFieldExploration.Enqueue((uint)VentureID);
        }
        if(ImGui.Button("选择"))
        {
            RetainerHandlers.SelectSpecificVenture((uint)VentureID);
        }
        if(ImGui.CollapsingHeader("雇员探险"))
        {
            var data = CSFramework.Instance()->UIModule->GetRaptureAtkModule()->AtkModule.GetStringArrayData(95);
            if(data != null)
            {
                for(var i = 0; i < data->AtkArrayData.Size; i++)
                {
                    var item = data->StringArray[i].Value;
                    if(item != null)
                    {
                        var str = MemoryHelper.ReadSeStringNullTerminated((nint)item);
                        ImGuiEx.Text($"{i}: {str.GetText()}");
                    }
                    else
                    {
                        ImGuiEx.Text($"{i}: 空");
                    }
                }
            }
        }

        if(ImGui.CollapsingHeader("获取可用探险名称"))
        {
            foreach(var x in VentureUtils.GetAvailableVentureNames())
            {
                ImGuiEx.Text($"{x}");
            }
        }
    }
}
