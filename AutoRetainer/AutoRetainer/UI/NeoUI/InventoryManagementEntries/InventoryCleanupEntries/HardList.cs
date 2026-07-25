using AutoRetainerAPI.Configuration;

namespace AutoRetainer.UI.NeoUI.InventoryManagementEntries.InventoryCleanupEntries;
public class HardList : InventoryManagementBase
{
    public override string Name => "背包清理/快速出售列表";
    private InventoryManagementCommon InventoryManagementCommon = new();

    private HardList()
    {
        Builder = InventoryCleanupCommon.CreateCleanupHeaderBuilder()
            .Section(Name)
            .TextWrapped("这些物品将始终被出售，不论其来源，只要堆叠数量不超过下方设置的数值。此外，仅这些物品会被出售给 NPC。")
            .InputInt(150f, $"可出售的最大堆叠数量", () => ref InventoryCleanupCommon.SelectedPlan.IMAutoVendorHardStackLimit)
            .Widget(() => InventoryManagementCommon.DrawListNew(
                itemId => InventoryCleanupCommon.SelectedPlan.AddItemToList(IMListKind.HardSell, itemId, out _),
                itemId => InventoryCleanupCommon.SelectedPlan.IMAutoVendorHard.Remove(itemId),
                InventoryCleanupCommon.SelectedPlan.IMAutoVendorHard, 
                (x) =>
                {
                    ImGui.SameLine();
                    ImGui.PushFont(UiBuilder.IconFont);
                    ImGuiEx.CollectionButtonCheckbox(FontAwesomeIcon.Database.ToIconString(), x, InventoryCleanupCommon.SelectedPlan.IMAutoVendorHardIgnoreStack);
                    ImGui.PopFont();
                    ImGuiEx.Tooltip($"忽略此物品的堆叠设置");
                },
                filter: item => item.PriceLow != 0))
            .Separator()
            .Widget(() =>
            {
                InventoryManagementCommon.ImportFromArDiscard(InventoryCleanupCommon.SelectedPlan.IMAutoVendorHard);
            });
    }
}
