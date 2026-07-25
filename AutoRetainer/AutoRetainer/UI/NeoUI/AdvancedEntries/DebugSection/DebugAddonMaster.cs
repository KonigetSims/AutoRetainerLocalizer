using ECommons.UIHelpers.AddonMasterImplementations;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;

namespace AutoRetainer.UI.NeoUI.AdvancedEntries.DebugSection;
public unsafe class DebugAddonMaster : DebugSectionBase
{
    public override void Draw()
    {
        if(ImGui.CollapsingHeader("雇员列表"))
        {
            if(TryGetAddonByName<AtkUnitBase>("雇员列表", out var addon) && IsAddonReady(addon))
            {
                var r = new AddonMaster.RetainerList(addon);
                foreach(var x in r.Retainers)
                {
                    ImGuiEx.Text($"{x.Name}, {x.IsActive}");
                    if(ImGuiEx.HoveredAndClicked())
                    {
                        x.Select();
                    }
                }
            }
        }

        if(ImGui.CollapsingHeader("标题菜单"))
        {
            if(TryGetAddonMaster<AddonMaster._TitleMenu>(out var m) && m.IsAddonReady)
            {
                ImGuiEx.Text($"就绪: {m.IsReady}");
                if(ImGui.Button("开始")) m.Start();
                if(ImGui.Button("数据中心")) m.DataCenter();
                if(ImGui.Button("退出")) m.Exit();
            }
        }

        if(ImGui.CollapsingHeader("标题数据中心世界地图"))
        {
            if(TryGetAddonMaster<AddonMaster.TitleDCWorldMap>(out var m) && m.IsAddonReady)
            {
                foreach(var x in AddonMaster.TitleDCWorldMap.PublicDC)
                {
                    if(ImGui.Button(Svc.Data.GetExcelSheet<WorldDCGroupType>().GetRowOrDefault((uint)x)?.Name.ToString() ?? ""))
                    {
                        m.Select(x);
                    }
                }
            }
        }

        if(ImGui.CollapsingHeader("角色选择世界服务器"))
        {
            if(TryGetAddonMaster<AddonMaster._CharaSelectWorldServer>(out var m))
            {
                foreach(var x in m.Worlds)
                {
                    if(ImGui.Button(x.Name))
                    {
                        x.Select();
                    }
                }
            }
        }

        if(ImGui.CollapsingHeader("角色选择列表菜单"))
        {
            if(TryGetAddonMaster<AddonMaster._CharaSelectListMenu>(out var m) && m.IsAddonReady)
            {
                if(ImGui.Button("世界##w"))
                {
                    m.SelectWorld();
                }
                //PluginLog.Information($"Chars: {m.Characters.Print("\n")}");
                ImGuiEx.Text($"{AgentLobby.Instance()->LobbyUpdateStage}");
                ImGuiEx.Text($"{AgentLobby.Instance()->HoveredCharacterContentId}");
                foreach(var x in m.Characters)
                {
                    if(ImGui.Button(x.ToString() + "/select"))
                    {
                        x.Select();
                    }
                    ImGui.SameLine();
                    if(ImGui.Button(x.ToString() + "/login"))
                    {
                        x.Login();
                    }
                    ImGui.SameLine();
                    if(ImGui.Button(x.ToString() + "/context"))
                    {
                        x.OpenContextMenu();
                    }
                    if(x.IsSelected)
                    {
                        ImGuiEx.Text($"已选择");
                    }
                }
            }
        }
    }
}
