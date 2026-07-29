using AutoRetainerAPI.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoRetainer.UI.NeoUI.InventoryManagementEntries.InventoryCleanupEntries;
public unsafe sealed class DiscardList : InventoryManagementBase
{
    public override string Name => "背包清理/丢弃列表";
    private InventoryManagementCommon InventoryManagementCommon = new();

    public override int DisplayPriority => -1;

    private DiscardList()
    {
        Builder = InventoryCleanupCommon.CreateCleanupHeaderBuilder()
            .Section(Name)
            .TextWrapped("这些物品将始终被丢弃，不论其来源为何，只要其堆叠数量不超过下方可设置的数量。丢弃动作会非常频繁地发生，会在每次可能改变背包的操作前后进行。丢弃优先级最高，即使同一物品也存在于出售或分解列表中，也会被丢弃。已设置为保护的物品不会被丢弃。")
            .InputInt(150f, $"最大丢弃堆叠数", () => ref InventoryCleanupCommon.SelectedPlan.IMDiscardStackLimit)
            .Widget(() => InventoryManagementCommon.DrawListNew(
                itemId => InventoryCleanupCommon.SelectedPlan.AddItemToList(IMListKind.Discard, itemId, out _),
                itemId => InventoryCleanupCommon.SelectedPlan.IMDiscardList.Remove(itemId),
                InventoryCleanupCommon.SelectedPlan.IMDiscardList,
                (x) =>
                {
                    ImGui.SameLine();
                    ImGui.PushFont(UiBuilder.IconFont);
                    ImGuiEx.CollectionButtonCheckbox(FontAwesomeIcon.Database.ToIconString(), x, InventoryCleanupCommon.SelectedPlan.IMDiscardIgnoreStack);
                    ImGui.PopFont();
                    ImGuiEx.Tooltip($"忽略此物品的堆叠设置");
                    ImGuiEx.DragDropRepopulate("StkStg", x, InventoryCleanupCommon.SelectedPlan.IMDiscardIgnoreStack);
                }))
            .Separator()
            .Widget(() =>
            {
                InventoryManagementCommon.ImportFromArDiscard(InventoryCleanupCommon.SelectedPlan.IMDiscardList);
            });
    }
}