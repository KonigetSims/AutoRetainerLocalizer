using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoRetainer.UI.NeoUI.InventoryManagementEntries.InventoryCleanupEntries;
public sealed unsafe class CleanupCharacterConfiguration : InventoryManagementBase
{
    public override string Name { get; } = "背包清理/角色配置";

    public override int DisplayPriority => -20;

    public override void Draw()
    {
        ImGuiEx.TextWrapped($"在这里可以将预设的背包清理列表分配给已注册角色。");
        ImGuiEx.SetNextItemFullWidth();
        ImGuiEx.FilteringInputTextWithHint("##search", "搜索...", out var filter);
        if(ImGuiEx.BeginDefaultTable(["~角色", "计划"]))
        {
            foreach(var characterData in C.OfflineData)
            {
                if(filter != "" && !characterData.NameWithWorld.Contains(filter, StringComparison.OrdinalIgnoreCase)) continue;
                ImGui.PushID(characterData.Identity);
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGuiEx.TextV(characterData.NameWithWorldCensored);
                ImGui.TableNextColumn();
                var plan = characterData.InventoryCleanupPlan == Guid.Empty ? null : C.AdditionalIMSettings.FirstOrDefault(p => p.GUID == characterData.InventoryCleanupPlan);
                ImGui.SetNextItemWidth(200f);
                if(ImGui.BeginCombo("##chPlan", plan?.DisplayName ?? "默认计划", ImGuiComboFlags.HeightLarge))
                {
                    if(ImGui.Selectable("默认计划", plan == null)) characterData.InventoryCleanupPlan = Guid.Empty;
                    ImGui.Separator();
                    foreach(var cleanupPlan in C.AdditionalIMSettings)
                    {
                        ImGui.PushID(cleanupPlan.ID);
                        if(ImGui.Selectable($"{cleanupPlan.DisplayName}"))
                        {
                            characterData.InventoryCleanupPlan = cleanupPlan.GUID;
                        }
                        ImGui.PopID();
                    }
                    ImGui.EndCombo();
                }
                ImGuiEx.DragDropRepopulate("清理计划", plan?.GUID ?? Guid.Empty, ref characterData.InventoryCleanupPlan);

                ImGui.PopID();
            }
            ImGui.EndTable();
        }
    }
}