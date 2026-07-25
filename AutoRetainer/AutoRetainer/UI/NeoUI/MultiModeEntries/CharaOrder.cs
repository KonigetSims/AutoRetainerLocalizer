using AutoRetainerAPI.Configuration;

namespace AutoRetainer.UI.NeoUI.MultiModeEntries;
public class CharaOrder : NeoUIEntry
{
    public override string Path => "多角色模式/排除与排序";

    private static string Search = "";
    private static ImGuiEx.RealtimeDragDrop<OfflineCharacterData> DragDrop = new("CharaOrder", x => x.Identity);

    public override bool NoFrame { get; set; } = true;

    public override void Draw()
    {
        C.OfflineData.RemoveAll(x => C.Blacklist.Any(z => z.CID == x.CID));
        var b = new NuiBuilder()
        .Section("角色排序")
        .Widget("在此处可对角色进行排序。这将影响多角色模式处理它们的顺序，以及它们在插件界面和登录覆盖层中的显示顺序。", (x) =>
        {
            ImGuiEx.TextWrapped($"在此处可对角色进行排序。这将影响多角色模式处理它们的顺序，以及它们在插件界面和登录覆盖层中的显示顺序。");
            ImGui.SetNextItemWidth(150f);
            ImGui.InputText($"搜索", ref Search, 50);
            DragDrop.Begin();
            if(ImGui.BeginTable("角色顺序表", 4, ImGuiTableFlags.Borders | ImGuiTableFlags.NoSavedSettings | ImGuiTableFlags.RowBg | ImGuiTableFlags.SizingFixedFit))
            {
                ImGui.TableSetupColumn("##ctrl");
                ImGui.TableSetupColumn("角色", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn("切换开关");
                ImGui.TableSetupColumn("删除数据");
                ImGui.TableHeadersRow();

                for(var index = 0; index < C.OfflineData.Count; index++)
                {
                    var chr = C.OfflineData[index];
                    ImGui.PushID(chr.Identity);
                    ImGui.TableNextRow();
                    DragDrop.SetRowColor(chr.Identity);
                    ImGui.TableNextColumn();
                    DragDrop.NextRow();
                    DragDrop.DrawButtonDummy(chr, C.OfflineData, index);
                    ImGui.TableNextColumn();
                    ImGuiEx.TextV((Search != "" && ($"{chr.Name}@{chr.World}").Contains(Search, StringComparison.OrdinalIgnoreCase)) ? ImGuiColors.ParsedGreen : (Search == "" ? null : ImGuiColors.DalamudGrey3), Censor.Character(chr.Name, chr.World));
                    ImGui.TableNextColumn();
                    if(ImGuiEx.ButtonCheckbox(FontAwesomeIcon.Users, ref chr.ExcludeRetainer, inverted: true))
                    {
                        chr.Enabled = false;
                        C.SelectedRetainers.Remove(chr.CID);
                    }
                    ImGuiEx.Tooltip("启用雇员自动化");
                    ImGuiEx.DragDropRepopulate("启用雇员", chr.ExcludeRetainer, ref chr.ExcludeRetainer);
                    ImGui.SameLine();
                    if(ImGuiEx.ButtonCheckbox(FontAwesomeIcon.Ship, ref chr.ExcludeWorkshop, inverted: true))
                    {
                        chr.WorkshopEnabled = false;
                        chr.EnabledSubs.Clear();
                        chr.EnabledAirships.Clear();
                    }
                    ImGuiEx.Tooltip("启用潜艇自动化");
                    ImGuiEx.DragDropRepopulate("启用派遣", chr.ExcludeWorkshop, x =>
                    {
                        chr.ExcludeWorkshop = x;
                        if(!x)
                        {
                            chr.EnabledSubs.Clear();
                            chr.EnabledAirships.Clear();
                        }
                    });
                    ImGui.SameLine();
                    ImGuiEx.ButtonCheckbox(FontAwesomeIcon.DoorOpen, ref chr.ExcludeOverlay, inverted: true);
                    ImGuiEx.Tooltip("在登录覆盖窗口显示");
                    ImGuiEx.DragDropRepopulate("启用日志", chr.ExcludeOverlay, ref chr.ExcludeOverlay);
                    ImGui.SameLine();
                    ImGuiEx.ButtonCheckbox(FontAwesomeIcon.Coins, ref chr.NoGilTrack, inverted: true);
                    ImGuiEx.Tooltip("将此角色的金币计入总额");
                    ImGuiEx.DragDropRepopulate("启用金币", chr.NoGilTrack, ref chr.NoGilTrack);
                    ImGui.SameLine();
                    ImGuiEx.ButtonCheckbox(FontAwesomeIcon.GasPump, ref chr.AutoFuelPurchase, color:ImGuiColors.TankBlue);
                    ImGuiEx.Tooltip("允许此角色在工坊购买燃料");
                    ImGuiEx.DragDropRepopulate("启用燃料", chr.AutoFuelPurchase, ref chr.AutoFuelPurchase);
                    ImGui.TableNextColumn();
                    if(ImGuiEx.IconButton(FontAwesomeIcon.UserMinus))
                    {
                        chr.ClearFCData();
                    }
                    ImGuiEx.Tooltip("重置此角色的部队数据与潜艇数据。数据将在你登录并访问控制面板后重新生成。");
                    ImGui.SameLine();
                    if(ImGuiEx.IconButton(FontAwesomeIcon.Trash, enabled: ImGuiEx.Ctrl))
                    {
                        new TickScheduler(() => C.OfflineData.Remove(chr));
                    }
                    ImGuiEx.Tooltip($"按住CTRL + 左键以删除存储的角色数据。重新登录后会自动重建。");
                    ImGui.SameLine();
                    if(ImGuiEx.IconButton("", enabled: ImGuiEx.Ctrl))
                    {
                        C.Blacklist.Add((chr.CID, chr.Name));
                    }
                    ImGuiEx.Tooltip($"按住CTRL + 左键以永久删除角色数据，该角色将完全排除在AutoRetainer的处理范围之外。");

                    ImGui.PopID();
                }

                ImGui.EndTable();
            }
            DragDrop.End();
        });


        if(C.Blacklist.Count != 0)
        {
            b = b.Section("已排除角色")
                .Widget(() =>
                {
                    for(var i = 0; i < C.Blacklist.Count; i++)
                    {
                        var d = C.Blacklist[i];
                        ImGuiEx.TextV($"{d.Name} ({d.CID:X16})");
                        ImGui.SameLine();
                        if(ImGui.Button($"删除##bl{i}"))
                        {
                            C.Blacklist.RemoveAt(i);
                            C.SelectedRetainers.Remove(d.CID);
                            break;
                        }
                    }
                });
        }

        b.Draw();
    }
}
