using AutoRetainer.Internal;
using AutoRetainer.Modules.Voyage.Readers;
using AutoRetainer.Scheduler.Tasks;
using AutoRetainer.UiHelpers;
using ECommons.UIHelpers.AtkReaderImplementations;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace AutoRetainer.UI.NeoUI.AdvancedEntries.DebugSection;

internal unsafe class DebugReader : DebugSectionBase
{
    public override void Draw()
    {
        {
            if(TryGetAddonByName<AtkUnitBase>("部队积分商店", out var a) && IsAddonReady(a))
            {
                var reader = new ReaderFreeCompanyCreditShop(a);
                ImGuiEx.Text($"""
                    军衔: {reader.FCRank}\n贡献: {reader.Credits}\n数量: {reader.Count}
                    """);
                for(var i = 0; i < reader.Count; i++)
                {
                    var x = reader.Listings[i];
                    ImGuiEx.Text($"{x}");
                    if(ImGuiEx.HoveredAndClicked()) new FreeCompanyCreditShop(a).Buy(0);
                    var amount = Math.Floor((float)reader.Credits / (float)(x.Price));
                }

                if(ImGui.Button("运行任务")) TaskRecursivelyBuyFuel.Enqueue();
            }
        }

        {
            if(TryGetAddonByName<AtkUnitBase>("雇员列表", out var a) && IsAddonReady(a))
            {
                var reader = new ReaderRetainerList(a);
                foreach(var x in reader.Retainers)
                {
                    ImGuiEx.Text($"{x.Name}/act {x.IsActive}/gil {x.Gil}/lvl {x.Level}/inv {x.Inventory}");
                }
            }
        }
        {
            if(TryGetAddonByName<AtkUnitBase>("雇员物品转移列表", out var a) && IsAddonReady(a))
            {
                var reader = new ReaderRetainerItemTransferList(a);
                foreach(var r in reader.Items)
                {
                    ImGuiEx.Text($"物品 {r.ItemID}, 是否为HQ = {r.IsHQ}");
                }
            }
        }
        {
            if(TryGetAddonByName<AtkUnitBase>("飞空艇探索", out var a) && IsAddonReady(a))
            {
                var reader = new ReaderAirShipExploration(a);
                ImGuiEx.Text($"距离: {reader.Distance}");
                ImGuiEx.Text($"燃料: {reader.Fuel}");
                foreach(var r in reader.Destinations)
                {
                    ImGuiEx.Text($"目的地 {r.NameFull}, 等级={r.RequiredRank}, 状态={r.StatusFlag}, 可否选择={r.CanBeSelected}");
                }
            }
        }
        {
            if(TryGetAddonByName<AtkUnitBase>("潜水艇探索地图选择", out var a) && IsAddonReady(a))
            {
                var reader = new ReaderSubmarineExplorationMapSelect(a);
                ImGuiEx.Text($"当前等级: {reader.SubmarineRank}");
                foreach(var r in reader.Maps)
                {
                    ImGuiEx.Text($"地图 {r.Name}, 等级={r.RequiredRank}");
                }
            }
        }
        {
            if(TryGetAddonByName<AtkUnitBase>("选择字符串", out var a) && IsAddonReady(a))
            {
                var reader = new ReaderSelectString(a);
                foreach(var r in reader.Entries)
                {
                    ImGuiEx.Text($"{r.Text}");
                }
            }
        }
    }
}
