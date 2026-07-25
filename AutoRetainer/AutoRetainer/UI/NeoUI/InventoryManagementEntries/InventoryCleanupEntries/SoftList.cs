using AutoRetainerAPI.Configuration;

namespace AutoRetainer.UI.NeoUI.InventoryManagementEntries.InventoryCleanupEntries;
public class SoftList : InventoryManagementBase
{
    public override string Name => "背包清理/快速雇员出售列表";
    private InventoryManagementCommon InventoryManagementCommon = new();
    private SoftList()
    {
        Builder = InventoryCleanupCommon.CreateCleanupHeaderBuilder()
            .Section(Name)
            .TextWrapped("这些物品从快速任务（Quick Venture）获得后会被出售，除非它们与相同物品堆叠。")
            .Widget(() => InventoryManagementCommon.DrawListNew(
                itemId => InventoryCleanupCommon.SelectedPlan.AddItemToList(IMListKind.SoftSell, itemId, out _),
                itemId => InventoryCleanupCommon.SelectedPlan.IMAutoVendorSoft.Remove(itemId), InventoryCleanupCommon.SelectedPlan.IMAutoVendorSoft,
                filter: item => item.PriceLow != 0))
            .Widget(() =>
            {
                InventoryManagementCommon.ImportFromArDiscard(InventoryCleanupCommon.SelectedPlan.IMAutoVendorSoft);
            });
    }
}
