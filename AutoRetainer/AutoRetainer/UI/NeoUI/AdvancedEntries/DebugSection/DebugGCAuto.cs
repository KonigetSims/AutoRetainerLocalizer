using ECommons.ExcelServices;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace AutoRetainer.UI.NeoUI.AdvancedEntries.DebugSection;

internal unsafe class DebugGCAuto : DebugSectionBase
{
    public override void Draw()
    {
        if(ImGui.CollapsingHeader("专家物品"))
        {
            foreach(var x in AutoGCHandin.GetHandinItems())
            {
                ImGuiEx.Text(x.ToString() + "/" + ExcelItemHelper.GetName(x.ItemID));
            }
        }
        if(ImGui.Button("入队初始化")) GCContinuation.EnqueueInitiation(true);
        if(ImGui.Button("入队交换关闭")) GCContinuation.EnqueueDeliveryClose();
        if(ImGui.Button("踏上")) P.TaskManager.StepMode = true;
        ImGui.SameLine();
        if(ImGui.Button("离开")) P.TaskManager.StepMode = false;
        ImGui.SameLine();
        if(ImGui.Button("步骤")) P.TaskManager.Step();
        if(ImGui.CollapsingHeader("军队补给列表"))
        {
            if(TryGetAddonByName<AtkUnitBase>("军队补给列表", out var addon) && IsAddonReady(addon))
            {
                var reader = new ReaderGrandCompanySupplyList(addon);
                if(reader.IsLoaded)
                {
                    var ptr = (GCExpertEntry*)*(nint*)((nint)addon + 648);
                    for(var i = 0; i < reader.NumItems; i++)
                    {
                        var entry = ptr[i];
                        ImGuiEx.Text($"{entry.Unk112}/{entry.Unk116}/{entry.Seals}/{entry.ItemID} {ExcelItemHelper.GetName(entry.ItemID)}/{entry.Unk136}/{entry.Unk145}");
                        ImGui.SameLine();
                        ImGuiEx.TextCopy($"{(nint)(&ptr[i])}");
                    }
                }
            }
        }
        if(ImGui.CollapsingHeader("军队交换"))
        {
            if(TryGetAddonByName<AtkUnitBase>("军队交换", out var addon) && IsAddonReady(addon))
            {
                var reader = new ReaderGrandCompanyExchange(addon);
                List<ImGuiEx.EzTableEntry> entries = [];
                foreach(var x in reader.Items)
                {
                    entries.Add(new("物品", () => ImGuiEx.TextCopy($"{x.Name}")));
                    entries.Add(new("ID", () => ImGuiEx.TextCopy($"{x.ItemID}")));
                    entries.Add(new("背包", () => ImGuiEx.TextCopy($"{x.Bag}")));
                    entries.Add(new("图标ID", () => ImGuiEx.TextCopy($"{x.IconID}")));
                    entries.Add(new("军衔要求", () => ImGuiEx.TextCopy($"{x.RankReq}")));
                    entries.Add(new("军票", () => ImGuiEx.TextCopy($"{x.Seals}")));
                    entries.Add(new("未知350", () => ImGuiEx.TextCopy($"{x.Unk350}")));
                    entries.Add(new("未知450", () => ImGuiEx.TextCopy($"{x.OpenCurrencyExchange}")));
                }
                ImGuiEx.EzTable(entries);
            }
        }
        ImGuiEx.Text($"获取军票倍率: {Utils.GetGCSealMultiplier()}");
        if(ImGui.Button(nameof(GCContinuation.SelectExchange))) DuoLog.Information($"{GCContinuation.SelectExchange()}");
        if(ImGui.Button(nameof(GCContinuation.ConfirmExchange))) DuoLog.Information($"{GCContinuation.ConfirmExchange()}");
        if(ImGui.Button(nameof(GCContinuation.SelectGCExchangeVerticalTab))) DuoLog.Information($"{GCContinuation.SelectGCExchangeVerticalTab(0)}");
        if(ImGui.Button(nameof(GCContinuation.SelectGCExchangeHorizontalTab))) DuoLog.Information($"{GCContinuation.SelectGCExchangeHorizontalTab(2)}");
        if(ImGui.Button(nameof(GCContinuation.InteractWithShop))) DuoLog.Information($"{GCContinuation.InteractWithShop()}");
        if(ImGui.Button(nameof(GCContinuation.InteractWithExchange))) DuoLog.Information($"{GCContinuation.InteractWithExchange()}");
        if(ImGui.Button(nameof(GCContinuation.SelectProvisioningMission))) DuoLog.Information($"{GCContinuation.SelectProvisioningMission()}");
        if(ImGui.Button(nameof(GCContinuation.SelectSupplyListTab))) DuoLog.Information($"{GCContinuation.SelectSupplyListTab(2)}");
        if(ImGui.Button(nameof(GCContinuation.EnableDeliveringIfPossible))) DuoLog.Information($"{GCContinuation.EnableDeliveringIfPossible()}");
        if(ImGui.Button(nameof(GCContinuation.CloseSupplyList))) DuoLog.Information($"{GCContinuation.CloseSupplyList()}");
        if(ImGui.Button(nameof(GCContinuation.CloseSelectString))) DuoLog.Information($"{GCContinuation.CloseSelectString()}");
        if(ImGui.Button(nameof(GCContinuation.CloseExchange))) DuoLog.Information($"{GCContinuation.CloseExchange()}");
        if(ImGui.Button(nameof(GCContinuation.OpenSeals))) DuoLog.Information($"{GCContinuation.OpenSeals()}");
    }
}
