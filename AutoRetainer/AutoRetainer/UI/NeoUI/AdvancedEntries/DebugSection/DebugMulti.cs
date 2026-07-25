using AutoRetainer.Internal;
using AutoRetainer.Scheduler.Tasks;
using Dalamud.Utility;
using ECommons.Automation.NeoTaskManager.Tasks;
using ECommons.ExcelServices;
using ECommons.ExcelServices.TerritoryEnumeration;
using ECommons.GameHelpers;
using ECommons.Reflection;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.Sheets;

namespace AutoRetainer.UI.NeoUI.AdvancedEntries.DebugSection;

internal unsafe class DebugMulti : DebugSectionBase
{
    public override void Draw()
    {
        ImGui.Checkbox("禁用渲染", ref P.TestRenderDisable);
        if(ImGui.CollapsingHeader("已排序数据"))
        {
            ImGuiEx.Text($"{MultiMode.GetRetainerSortedOfflineDatas(true).Where(x => !x.ExcludeRetainer).Select(x => $"{x.Name}@{x.World}").Print("\n")}");
        }
        if(ImGui.CollapsingHeader("新HET"))
        {
            if(ImGui.Button("入队 HET")) TaskNeoHET.Enqueue(null);
            if(ImGui.Button("入队工坊")) TaskNeoHET.TryEnterWorkshop(() => DuoLog.Error("失败"));
            ImGuiEx.Text($"""
                能否进入工坊: {Lifestream.CanMoveToWorkshop()}
                """);
        }
        if(ImGui.CollapsingHeader("任务"))
        {
            if(ImGui.Button("测试自动移动任务")) P.TaskManager.EnqueueTask(NeoTasks.ApproachObjectViaAutomove(() => Svc.Targets.FocusTarget));
            if(ImGui.Button("测试交互任务")) P.TaskManager.EnqueueTask(NeoTasks.InteractWithObject(() => Svc.Targets.FocusTarget));
            if(ImGui.Button("测试两者"))
            {
                P.TaskManager.EnqueueTask(NeoTasks.ApproachObjectViaAutomove(() => Svc.Targets.FocusTarget));
                P.TaskManager.EnqueueTask(NeoTasks.InteractWithObject(() => Svc.Targets.FocusTarget));
            }
        }
        ImGui.Checkbox("不登出", ref C.DontLogout);
        ImGui.Checkbox("启用", ref MultiMode.Enabled);
        ImGuiEx.Text($"期望值: {MultiMode.ExpectedCharacter}");
        if(ImGui.Button("强制不匹配")) MultiMode.ExpectedCharacter = ("AAAAAAAA", "BBBBBBB");
        if(ImGui.Button("模拟无剩余"))
        {
            MultiMode.Relog(null, out var error, RelogReason.MultiMode);
        }
        if(ImGui.Button($"模拟自动启动"))
        {
            MultiMode.PerformAutoStart();
        }
        if(ImGui.Button("删除已加载数据"))
        {
            DalamudReflector.DeleteSharedData("AutoRetainer.WasLoaded");
        }
        ImGuiEx.Text($"移动中: {AgentMap.Instance()->IsPlayerMoving}");
        ImGuiEx.Text($"占用中: {IsOccupied()}");
        ImGuiEx.Text($"咏唱中: {Player.Object?.IsCasting}");
        ImGuiEx.TextCopy($"CID: {Player.CID}");
        ImGuiEx.Text($"{Svc.Data.GetExcelSheet<Addon>()?.GetRow(115).Text.ToDalamudString().GetText()}");
        ImGuiEx.Text($"服务器时间: {CSFramework.GetServerTime()}");
        ImGuiEx.Text($"电脑时间: {DateTimeOffset.Now.ToUnixTimeSeconds()}");
        if(ImGui.CollapsingHeader("HET"))
        {
            ImGuiEx.Text($"最近入口: {Utils.GetNearestEntrance(out var d)}, 距离={d}");
            if(ImGui.Button("进入房屋"))
            {
                TaskNeoHET.Enqueue(null);
            }
        }
        if(ImGui.CollapsingHeader("房屋区域"))
        {
            ImGuiEx.Text(ResidentalAreas.List.Select(x => GenericHelpers.GetTerritoryName(x)).Join("\n"));
            ImGuiEx.Text($"在住宅区中: {ResidentalAreas.List.Contains((ushort)Svc.ClientState.TerritoryType)}");
        }
        ImGuiEx.Text($"是否在安全区: {TerritoryInfo.Instance()->InSanctuary}");
        ImGuiEx.Text($"是否在安全区(Excel): {ExcelTerritoryHelper.IsSanctuary(Svc.ClientState.TerritoryType)}");
        ImGui.Checkbox($"绕过安全区检查", ref C.BypassSanctuaryCheck);
        if(Svc.ClientState.LocalPlayer != null && Svc.Targets.Target != null)
        {
            ImGuiEx.Text($"到目标距离: {Vector3.Distance(Svc.ClientState.LocalPlayer.Position, Svc.Targets.Target.Position)}");
            ImGuiEx.Text($"目标命中框: {Svc.Targets.Target.HitboxRadius}");
            ImGuiEx.Text($"到目标命中框距离: {Vector3.Distance(Svc.ClientState.LocalPlayer.Position, Svc.Targets.Target.Position) - Svc.Targets.Target.HitboxRadius}");
        }
        if(ImGui.CollapsingHeader("角色选择"))
        {
            foreach(var x in Utils.GetCharacterNames())
            {
                ImGuiEx.Text($"{x.Name}@{x.World}");
            }
        }
    }
}
