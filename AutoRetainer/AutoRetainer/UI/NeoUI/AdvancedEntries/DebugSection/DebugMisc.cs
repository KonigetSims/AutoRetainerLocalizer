using AutoRetainer.Modules.Voyage;
using AutoRetainer.Scheduler.Tasks;
using Dalamud.Utility;
using ECommons.Configuration;
using ECommons.Events;
using ECommons.ExcelServices;
using ECommons.GameFunctions;
using ECommons.Interop;
using ECommons.IPC;
using ECommons.MathHelpers;
using ECommons.Throttlers;
using ECommons.UIHelpers.AddonMasterImplementations;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using ItemLevel = AutoRetainer.Helpers.ItemLevel;

namespace AutoRetainer.UI.NeoUI.AdvancedEntries.DebugSection;

internal unsafe class DebugMisc : DebugSectionBase
{
    public override void Draw()
    {
        if(ImGui.Button("EnableSingleMultiMode")) ECommonsIPC.AutoRetainer.EnableSingleMultiMode(null);
        if(ImGui.Button("EnableSingleMultiMode (subs)")) ECommonsIPC.AutoRetainer.EnableSingleMultiMode(ECommons.IPC.Subscribers.AutoRetainer.MultiModeType.Submersibles);
        if(ImGui.CollapsingHeader("Move stuck detection"))
        {
            ImGuiEx.Text($"""
                
                LastRefreshTime {BailoutManager.LastRefreshTime}({Environment.TickCount64 - BailoutManager.LastRefreshTime})
                LastPosition {BailoutManager.LastPosition}
                ExtendIsMoving {EzThrottler.GetRemainingTime("ExtendIsMoving")}
                """);
        }
        if(ImGui.CollapsingHeader("AddonOpenedAt"))
        {
            foreach(var x in BailoutManager.AddonOpenedAt.Keys)
            {
                ImGuiEx.Text($"{x}: {BailoutManager.AddonOpenedAt[x]} ({Environment.TickCount64 - BailoutManager.AddonOpenedAt[x]} ago)");
                ImGui.SameLine();
                if(ImGui.SmallButton($"-1 min##{x}"))
                {
                    BailoutManager.AddonOpenedAt[x] -= 60 * 1000;
                }
            }
        }
        ImGui.Checkbox("渲染禁用详细日志", ref RenderDisableManager.Debug);
        if(ImGui.Button("执行插件终止器"))
        {
            S.PluginTerminator.OnUpdate();
        }
        ImGuiEx.Text($"离线潜艇数据.计数 {Data.OfflineSubmarineData.Count}, 潜艇栏位数 {Data.NumSubSlots}");
        ImGuiEx.Text($"部队等级: {Utils.FCRank}");
        if(ImGui.CollapsingHeader("API测试1"))
        {
            try
            {
                ImGuiEx.Text($"{P.API.Config.FCData}");
            }
            catch(Exception e)
            {
                ImGuiEx.Text($"{e}");
            }
        }
        if(ImGuiEx.Button("传送"))
        {
            MultiMode.RunTeleportLogic();
        }
        if(ImGui.CollapsingHeader("询问资格"))
        {
            ImGuiEx.Text($"""
                当前角色：\n已派遣雇员探险：{Data?.SentVenturesByDay.Sum(x => x.Value)}\n已派遣潜艇航行：{Data?.SentVoyagesByDay.Sum(x => x.Value)}\n启用的雇员上限：{Data?.GetEnabledRetainers(false).Length}\n总计已派遣探险：{C.OfflineData.Sum(x => x.SentVenturesByDay.Select(x => x.Value).Sum())}\n总计已派遣航行：{C.OfflineData.Sum(x => x.SentVoyagesByDay.Select(x => x.Value).Sum())}\n全局启用的雇员总量：{C.OfflineData.Select(x => x.GetEnabledRetainers().Length).MaxSafe()}\n已启用雇员自动化的角色数：{C.OfflineData.Where(x => x.GetEnabledRetainers().Length > 0 && x.Enabled).Count()}\n已启用潜艇自动化的角色数：{C.OfflineData.Where(x => x.GetEnabledVesselsData(Internal.VoyageType.Submersible).Count > 0 && x.WorkshopEnabled).Count()}\n---------\n按日期统计:
                """);
            var days = C.OfflineData.Select(x => (long[])[..x.SentVenturesByDay.Keys, ..x.SentVoyagesByDay.Keys]).SelectNested(x => x).ToHashSet();
            ImGui.Indent();
            foreach(var x in days)
            {
                ImGuiEx.Text($"{x}: 探险次数: {C.OfflineData.Select(c => c.SentVenturesByDay.SafeSelect(x)).Sum()}, 航行次数: {C.OfflineData.Select(c => c.SentVoyagesByDay.SafeSelect(x)).Sum()}");
            }
            ImGui.Unindent();
            ImGuiEx.Text($"""
                ---------\n按角色统计:
                """);
            foreach(var x in C.OfflineData)
            {
                ImGuiEx.Text($"{x.NameWithWorld}: 探险次数: {x.SentVenturesByDay.Sum(s => s.Value)}, 航行次数: {x.SentVoyagesByDay.Sum(s => s.Value)}");
            }
        }
        if(ImGui.CollapsingHeader("部队行动"))
        {
            ImGuiEx.Text($"数量: {TaskActivateSealSweetener.NumActions}");
            foreach(var x in TaskActivateSealSweetener.Actions)
            {
                ImGuiEx.Text($"{x} / {Svc.Data.GetExcelSheet<CompanyAction>().GetRowOrDefault((uint)x)?.Name}");
            }
            ImGuiEx.FilteringInputInt("回调值 1", out var val1);
            ImGuiEx.FilteringInputInt("回调值 2", out var val2);
            if(ImGui.Button("在部队界面"))
            {
                if(TryGetAddonByName<AtkUnitBase>("部队", out var addon) && addon->IsReady())
                {
                    Callback.Fire(addon, true, val1, (uint)val2);
                }
            }
            if(ImGui.Button("在部队行动界面"))
            {
                if(TryGetAddonByName<AtkUnitBase>("部队行动", out var addon) && addon->IsReady())
                {
                    Callback.Fire(addon, true, val1, (uint)val2);
                }
            }
            if(ImGui.Button("任务激活军票加成.入队"))
            {
                TaskActivateSealSweetener.Enqueue();
            }
            if(ImGui.Button("任务激活军票加成.限流入队"))
            {
                TaskActivateSealSweetener.EnqueueThrottled();
            }
        }
        if(ImGui.CollapsingHeader("618"))
        {
            var a = Svc.Data.GetExcelSheet<Lobby>().GetRow(618).Text.ToDalamudString();
            foreach(var pl in a.Payloads)
            {
                ImGuiEx.Text($"{pl.Type}: {pl.ToString()}");
            }
        }
        if(ImGui.CollapsingHeader("上下文菜单"))
        {
            if(TryGetAddonMaster<AddonMaster.ContextMenu>(out var m) && m.IsAddonReady)
            {
                foreach(var x in m.Entries)
                {
                    ImGuiEx.Text($"{x.Text}/{x.Enabled}");
                }
            }
        }
        if(ImGui.CollapsingHeader("雇员物品属性"))
        {
            var im = InventoryManager.Instance();
            var c = im->GetInventoryContainer(InventoryType.RetainerEquippedItems);
            for(var i = 0; i < c->Size; i++)
            {
                var slot = c->GetInventorySlot(i);
                ImGuiEx.Text($"{i} ({slot->GetItemId()}): {ExcelItemHelper.GetName(slot->GetItemId() % 1000000)}, 获得力: {slot->GetStat(BaseParamEnum.Gathering)} [{slot->GetStatCap(BaseParamEnum.Gathering)}], 鉴别力: {slot->GetStat(BaseParamEnum.Perception)} [{slot->GetStatCap(BaseParamEnum.Perception)}]");
            }
        }
        if(ImGui.Button("测试 Haseltweaks"))
        {
            Utils.EnsureEnhancedLoginIsOff();
        }
        if(ImGui.Button("通过外部进程写入配置"))
        {
            ExternalWriter.PlaceWriteOrder(new(System.IO.Path.Combine(Svc.PluginInterface.ConfigDirectory.FullName, "WriterTest.json"), EzConfig.DefaultSerializationFactory.Serialize(C, true)));
        }
        ImGuiEx.Text($"部队战绩: {Utils.FCPoints}");
        if(ImGui.CollapsingHeader("房屋"))
        {
            var h = HousingManager.Instance();
            ImGuiEx.Text($"获取当前分区 {h->GetCurrentDivision()}");
            ImGuiEx.Text($"获取当前房屋ID {h->GetCurrentIndoorHouseId()}");
            ImGuiEx.Text($"获取当前地块 {h->GetCurrentPlot()}");
            ImGuiEx.Text($"获取当前房间 {h->GetCurrentRoom()}");
            ImGuiEx.Text($"获取当前小区 {h->GetCurrentWard()}");
            if(ImGui.Button("模拟登录"))
            {
                ProperOnLogin.FireArtificially();
            }
            if(h->OutdoorTerritory != null)
            {
                for(var i = 0; i < 30; i++)
                {
                    ImGuiEx.Text($"是否为房屋住户 {i}: {P.Memory.OutdoorTerritory_IsEstateResident((nint)h->OutdoorTerritory, (byte)i)}");
                }
            }
        }
        if(ImGui.Button("安装回调钩子")) Callback.InstallHook();
        if(ImGui.Button("禁用回调钩子")) Callback.UninstallHook();
        ImGuiEx.TextCopy($"{(nint)(&TargetSystem.Instance()->Target):X16}");
        ImGui.Checkbox($"记录操作码", ref P.LogOpcodes);
        ImGuiEx.Text($"CSFramework.Instance()->帧计数器: {CSFramework.Instance()->FrameCounter}");
        if(ImGui.Button("测试存放重复"))
        {
            if(TryGetAddonByName<AtkUnitBase>("雇员物品转移列表", out var addon))
            {
                Callback.Fire(addon, true, 0, (uint)29);
            }
        }
        ImGuiEx.Text($"锁定: {*(byte*)((nint)TargetSystem.Instance() + 309)}");
        if(ImGui.Button("冻结帧锁定"))
        {
            FPSManager.LockChillFrames();
        }
        if(ImGui.Button("取消帧锁定"))
        {
            FPSManager.UnlockChillFrames();
        }
        ImGui.Separator();
        ImGuiEx.Text($"CSFramework.Instance()->窗口非活动: {CSFramework.Instance()->WindowInactive}");
        ImGuiEx.Text($"按键是否按下(C.TempCollectB): {IsKeyPressed(C.TempCollectB)}");
        ImGuiEx.Text($"位掩码检测(User32.GetKeyState((int)C.TempCollectB), 15): {Bitmask.IsBitSet(TerraFX.Interop.Windows.Windows.GetKeyState((int)C.TempCollectB), 15)}");
        ImGuiEx.Text($"不重新分配: {C.DontReassign}, 按键 {C.TempCollectB}/{(int)C.TempCollectB}");
        foreach(var x in C.OfflineData)
        {
            ImGuiEx.Text($"{x.Name}@{x.World}: {x.Gil + x.RetainerData.Sum(z => z.Gil)}");
        }
        var ocd = Data;
        if(ocd != null)
        {
            ImGuiEx.Text($"等级数组:");
            ImGuiEx.Text(ocd.ClassJobLevelArray.Print());
        }

        ImGuiEx.Text($"{Utils.TryGetCurrentRetainer(out var n)}/{n}");
        ImGuiEx.Text($"{ItemLevel.Calculate(out var g, out var p)}/{g}/{p}");
        if(ImGui.Button("重新生成隐私种子"))
        {
            C.CensorSeed = Guid.NewGuid().ToString();
        }
        var inv = Utils.GetActiveRetainerInventoryName();
        ImGuiEx.Text($"Utils.GetActiveRetainerInventoryName(): {inv.Name} {inv.EntrustDuplicatesIndex}");
        ImGuiEx.Text($"条件曾被启用={P.ConditionWasEnabled}");
        if(ImGui.CollapsingHeader("任务调试"))
        {
            ImGuiEx.Text($"忙碌: {P.TaskManager.IsBusy}, 中止于 {P.TaskManager.RemainingTimeMS}");
            if(ImGui.Button($"生成随机数 1/500"))
            {
                P.TaskManager.Enqueue(() => { var r = new Random().Next(0, 500); InternalLog.Verbose($"生成 1/500: {r}"); return r == 0; });
            }
            if(ImGui.Button($"生成随机数 1/5000"))
            {
                P.TaskManager.Enqueue(() => { var r = new Random().Next(0, 5000); InternalLog.Verbose($"生成 1/5000: {r}"); return r == 0; });
            }
            if(ImGui.Button($"生成随机数 1/100"))
            {
                P.TaskManager.Enqueue(() => { var r = new Random().Next(0, 100); InternalLog.Verbose($"生成 1/100: {r}"); return r == 0; });
            }
        }
        ImGuiEx.Text($"QSI 状态: {P.quickSellItems?.openInventoryContextHook?.IsEnabled}");
        ImGuiEx.Text($"QuickSellItems.IsReadyToUse: {QuickSellItems.IsReadyToUse()}");

        foreach(var x in S.VentureStats.CharTotal)
        {
            ImGuiEx.Text($"{x.Key} : {x.Value}");
        }
        foreach(var x in S.VentureStats.RetTotal)
        {
            ImGuiEx.Text($"{x.Key} : {x.Value}");
        }

        ImGui.Separator();
        {
            if(ImGui.Button("触发") && TryGetAddonByName<AtkUnitBase>("军队补给列表", out var addon) && IsAddonReady(addon) && addon->UldManager.NodeList[5]->IsVisible())
            {
                AutoGCHandin.InvokeHandin(addon, 0);
            }
        }

        {
            if(TryGetAddonByName<AtkUnitBase>("军队补给列表", out var addon) && IsAddonReady(addon))
            {
                ImGuiEx.Text($"选中的筛选器是否有效: {AutoGCHandin.IsSelectedFilterValid(addon)}");
            }
        }

    }
}
