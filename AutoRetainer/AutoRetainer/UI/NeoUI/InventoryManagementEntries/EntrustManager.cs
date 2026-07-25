using AutoRetainerAPI.Configuration;
using ECommons.Configuration;
using ECommons.ExcelServices;
using ECommons.Reflection;
using ECommons.Throttlers;
using Lumina.Excel.Sheets;

namespace AutoRetainer.UI.NeoUI.InventoryManagementEntries;
public class EntrustManager : InventoryManagementBase
{
    public override string Name { get; } = "委托管理";
    private Guid SelectedGuid = Guid.Empty;
    private string Filter = "";
    private InventoryManagementCommon InventoryManagementCommon = new();

    public override void Draw()
    {
        ImGuiEx.TextWrapped("使用进阶存放管理器将特定物品存入特定雇员。在此窗口中可设置具体存放方案；随后，您可在雇员配置窗口中将存放方案分配给雇员。");
        ImGui.Checkbox("启用", ref C.EnableEntrustManager);
        ImGui.Checkbox("将委托的物品输出至聊天频道", ref C.EnableEntrustChat);
        var selectedPlan = C.EntrustPlans.FirstOrDefault(x => x.Guid == SelectedGuid);

        ImGuiEx.InputWithRightButtonsArea(() =>
        {
            if(ImGui.BeginCombo($"##select", selectedPlan?.Name ?? "选择方案...", ImGuiComboFlags.HeightLarge))
            {
                for(var i = 0; i < C.EntrustPlans.Count; i++)
                {
                    var plan = C.EntrustPlans[i];
                    ImGui.PushID(plan.Guid.ToString());
                    if(ImGui.Selectable(plan.Name, plan == selectedPlan))
                    {
                        SelectedGuid = plan.Guid;
                    }
                    ImGui.PopID();
                }
                ImGui.EndCombo();
            }
        }, () =>
        {
            if(ImGuiEx.IconButton(FontAwesomeIcon.Plus))
            {
                var plan = new EntrustPlan();
                C.EntrustPlans.Add(plan);
                SelectedGuid = plan.Guid;
                plan.Name = $"存放计划 {C.EntrustPlans.Count}";
            }
            ImGui.SameLine();
            if(ImGuiEx.IconButton(FontAwesomeIcon.Trash, enabled: selectedPlan != null && ImGuiEx.Ctrl))
            {
                C.EntrustPlans.Remove(selectedPlan);
            }
            ImGuiEx.Tooltip("按住 CTRL 并点击");
            ImGui.SameLine();
            if(ImGuiEx.IconButton(FontAwesomeIcon.Copy, enabled: selectedPlan != null))
            {
                Copy(EzConfig.DefaultSerializationFactory.Serialize(selectedPlan, false));
            }
            ImGui.SameLine();
            if(ImGuiEx.IconButton(FontAwesomeIcon.Paste, enabled: EzThrottler.Check("导入计划")))
            {
                try
                {
                    var plan = EzConfig.DefaultSerializationFactory.Deserialize<EntrustPlan>(Paste()) ?? throw new NullReferenceException();
                    plan.Guid = Guid.NewGuid();
                    if(plan.GetType().GetFieldPropertyUnions(ReflectionHelper.AllFlags).Any(x => x.GetValue(plan) == null)) throw new NullReferenceException();
                    C.EntrustPlans.Add(plan);
                    SelectedGuid = plan.Guid;
                    Notify.Success("已从剪贴板导入计划");
                    EzThrottler.Throttle("导入计划", 2000, true);
                }
                catch(Exception e)
                {
                    DuoLog.Error(e.Message);
                }
            }
        });
        if(selectedPlan != null)
        {
            ImGuiEx.SetNextItemFullWidth();
            ImGui.InputTextWithHint($"##name", "计划名称", ref selectedPlan.Name, 100);
            ImGui.Checkbox("存放重复物品", ref selectedPlan.Duplicates);
            ImGuiEx.HelpMarker("模拟游戏原生的存放重复物品功能：将你身上已存在于雇员背包中的物品移交过去，直到该物品堆叠满为止。此功能不影响水晶。加入下方列表的物品或类别将不会被此选项处理。");
            ImGui.Indent();
            ImGui.Checkbox("允许超过堆叠上限", ref selectedPlan.DuplicatesMultiStack);
            ImGuiEx.HelpMarker("允许在存放重复物品时，于所选雇员背包中创建该物品的新堆叠。");
            ImGui.Unindent();
            ImGui.Checkbox("允许从装备库存放物品", ref selectedPlan.AllowEntrustFromArmory);
            ImGui.Checkbox("仅限手动执行", ref selectedPlan.ManualPlan);
            ImGuiEx.HelpMarker("将此计划标记为仅限手动执行。此计划只会在手动点击 \"存放物品\" 按钮时执行，绝不会自动运行。");
            ImGui.Checkbox("排除存在于保护列表中的物品", ref selectedPlan.ExcludeProtected);
            ImGui.Separator();
            ImGuiEx.TreeNodeCollapsingHeader($"存放类别 (已选择{selectedPlan.EntrustCategories.Count} 个)###ecats", () =>
            {
                ImGuiEx.TextWrapped($"你可以在此选择要整类存放的物品类别。下方单独选取的特定物品将不受此规则限制。");
                if(ImGui.BeginTable("存放列表表格", 3, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.NoSavedSettings | ImGuiTableFlags.BordersInner))
                {
                    ImGui.TableSetupColumn("##1");
                    ImGui.TableSetupColumn("物品名称", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableSetupColumn("保留数量");
                    ImGui.TableHeadersRow();
                    foreach(var x in Svc.Data.GetExcelSheet<ItemUICategory>())
                    {
                        if(x.Name == "" || x.RowId == 39) continue;
                        var contains = selectedPlan.EntrustCategories.Any(s => s.ID == x.RowId);
                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        if(ThreadLoadImageHandler.TryGetIconTextureWrap(x.Icon, true, out var icon))
                        {
                            ImGui.Image(icon.Handle, new(ImGui.GetFrameHeight()));
                        }
                        ImGui.TableNextColumn();
                        if(ImGui.Checkbox(x.Name.ToString(), ref contains))
                        {
                            if(contains)
                            {
                                selectedPlan.EntrustCategories.Add(new() { ID = x.RowId });
                            }
                            else
                            {
                                selectedPlan.EntrustCategories.RemoveAll(s => s.ID == x.RowId);
                            }
                        }
                        ImGui.TableNextColumn();
                        if(selectedPlan.EntrustCategories.TryGetFirst(s => s.ID == x.RowId, out var result))
                        {
                            ImGui.SetNextItemWidth(130f);
                            ImGui.InputInt($"##amtkeep{result.ID}", ref result.AmountToKeep);
                        }
                    }
                    ImGui.EndTable();
                }
            });
            ImGuiEx.TreeNodeCollapsingHeader($"存放单一物品 (已选择{selectedPlan.EntrustItems.Count} 个)###eitems", () =>
            {
                InventoryManagementCommon.DrawListNew(
                    itemId => selectedPlan.EntrustItems.Add(itemId), 
                    itemId => selectedPlan.EntrustItems.Remove(itemId), 
                    selectedPlan.EntrustItems, (x) =>
                {
                    var amount = selectedPlan.EntrustItemsAmountToKeep.SafeSelect(x);
                    ImGui.SameLine();
                    ImGui.SetNextItemWidth(130f);
                    if(ImGui.InputInt($"##amtkeepitem{x}", ref amount))
                    {
                        selectedPlan.EntrustItemsAmountToKeep[x] = amount;
                    }
                    ImGuiEx.Tooltip("保留在自己背包中的数量");
                });
            });
            ImGuiEx.TreeNodeCollapsingHeader($"快速添加/移除", () =>
            {
                ImGuiEx.TextWrapped(GradientColor.Get(EColor.RedBright, EColor.YellowBright), $"当此文字可见时，将鼠标悬停在物品上并按住按键:");
                ImGuiEx.Text(!ImGui.GetIO().KeyShift ? ImGuiColors.DalamudGrey : ImGuiColors.DalamudRed, $"Shift - 添加至存入计划");
                ImGuiEx.Text(!ImGui.GetIO().KeyAlt ? ImGuiColors.DalamudGrey : ImGuiColors.DalamudRed, $"Alt - 从委托方案删除");
                if(Svc.GameGui.HoveredItem > 0)
                {
                    var id = (uint)(Svc.GameGui.HoveredItem % 1000000);
                    if(ImGui.GetIO().KeyShift)
                    {
                        if(!selectedPlan.EntrustItems.Contains(id))
                        {
                            selectedPlan.EntrustItems.Add(id);
                            Notify.Success($"已将 {ExcelItemHelper.GetName(id)} 加入存放计划 {selectedPlan.Name}");
                        }
                    }
                    if(ImGui.GetIO().KeyAlt)
                    {
                        if(selectedPlan.EntrustItems.Contains(id))
                        {
                            selectedPlan.EntrustItems.Remove(id);
                            Notify.Success($"已将 {ExcelItemHelper.GetName(id)} 从存放计划 {selectedPlan.Name} 移除");
                        }
                    }
                }
            });
        }
    }
}
