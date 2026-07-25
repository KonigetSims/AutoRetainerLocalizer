using ECommons.ExcelServices;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Text;
using TerraFX.Interop.Windows;
using Cabinet = Lumina.Excel.Sheets.Cabinet;

namespace AutoRetainer.UI.NeoUI.AdvancedEntries.DebugSection;

public unsafe class DebugCabinet : DebugSectionBase
{
    public override void Draw()
    {
        ImGuiEx.Text($"能否交付衣柜: {S.CabinetManager.CanDeliverCabinet()}");
        if(ImGui.Button("交付物品")) S.CabinetManager.EnqueueAllDeliverableItems();
        if(ImGui.Button("入队前往旅馆并交付所有物品")) S.CabinetManager.EnqueueGoToInnAndDeliverEverything();
        if(S.CabinetManager.TryGetStoredCabinetItems(out var cached, out var items))
        {
            ImGuiEx.Text($"已缓存: {cached}");
        }
    }
}
