using AutoRetainer.Scheduler.Handlers;
using AutoRetainer.Scheduler.Tasks;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;

namespace AutoRetainer.UI.NeoUI.AdvancedEntries.DebugSection;

internal unsafe class DebugScheduler : DebugSectionBase
{
    private string dbgRetName = string.Empty;
    public override void Draw()
    {
        ImGuiEx.Text($"金币: {TaskDepositGil.Gil}");
        ImGui.Checkbox($"任务提取金币.强制检查", ref TaskWithdrawGil.forceCheck);
        ImGuiEx.Text($"{Svc.Data.GetExcelSheet<LogMessage>().GetRow(4578).Text.ToDalamudString().GetText(true)}");
        if(ImGui.Button("关闭雇员菜单"))
        {
            DuoLog.Information($"{RetainerHandlers.CloseAgentRetainer()}");
        }
        ImGuiEx.Text($"Utils.AnyRetainersAvailableCurrentChara(): {Utils.AnyRetainersAvailableCurrentChara()}");
        if(ImGui.Button($"选择分配探险"))
        {
            DuoLog.Information($"{RetainerHandlers.SelectAssignVenture()}");
        }
        if(ImGui.Button($"选择退出"))
        {
            DuoLog.Information($"{RetainerHandlers.SelectQuit()}");
        }
        if(ImGui.Button($"选择查看探险报告"))
        {
            DuoLog.Information($"{RetainerHandlers.SelectViewVentureReport()}");
        }
        if(ImGui.Button($"点击结果重新分配"))
        {
            DuoLog.Information($"{RetainerHandlers.ClickResultReassign()}");
        }
        if(ImGui.Button($"点击结果确认"))
        {
            DuoLog.Information($"{RetainerHandlers.ClickResultConfirm()}");
        }
        if(ImGui.Button($"点击询问分配"))
        {
            DuoLog.Information($"{RetainerHandlers.ClickAskAssign()}");
        }
        if(ImGui.Button($"选择快速探索"))
        {
            DuoLog.Information($"{RetainerHandlers.SelectQuickExploration()}");
        }
        if(ImGui.Button($"选择委托物品"))
        {
            DuoLog.Information($"{RetainerHandlers.SelectEntrustItems()}");
        }
        if(ImGui.Button($"选择委托金币"))
        {
            DuoLog.Information($"{RetainerHandlers.SelectEntrustGil()}");
        }
        if(ImGui.Button($"点击存放重复物品"))
        {
            DuoLog.Information($"{RetainerHandlers.ClickEntrustDuplicates()}");
        }
        if(ImGui.Button($"点击确认存放重复物品"))
        {
            DuoLog.Information($"{RetainerHandlers.ClickEntrustDuplicatesConfirm()}");
        }
        if(ImGui.Button($"点击关闭委托窗口"))
        {
            DuoLog.Information($"{RetainerHandlers.ClickCloseEntrustWindow()}");
        }
        if(ImGui.Button($"关闭雇员背包"))
        {
            DuoLog.Information($"{RetainerHandlers.CloseAgentRetainer()}");
        }
        if(ImGui.Button($"关闭雇员背包"))
        {
            DuoLog.Information($"{RetainerHandlers.CloseAgentRetainer()}");
        }
        if(ImGui.Button($"设置提取金币金额 (1%)"))
        {
            DuoLog.Information($"{RetainerHandlers.SetWithdrawGilAmount(1)}");
        }
        if(ImGui.Button($"设置提取金币金额 (50%)"))
        {
            DuoLog.Information($"{RetainerHandlers.SetWithdrawGilAmount(50)}");
        }
        if(ImGui.Button($"设置提取金币金额 (99%)"))
        {
            DuoLog.Information($"{RetainerHandlers.SetWithdrawGilAmount(99)}");
        }
        if(ImGui.Button($"设置提取金币金额 (100%)"))
        {
            DuoLog.Information($"{RetainerHandlers.SetWithdrawGilAmount(100)}");
        }
        if(ImGui.Button($"提取金币或取消"))
        {
            DuoLog.Information($"{RetainerHandlers.ProcessBankOrCancel()}");
        }
        if(ImGui.Button($"提取金币或取消 (强制取消)"))
        {
            DuoLog.Information($"{RetainerHandlers.ProcessBankOrCancel(true)}");
        }
        if(ImGui.Button($"切换银行模式"))
        {
            DuoLog.Information($"{RetainerHandlers.SwapBankMode()}");
        }
        if(ImGui.Button($"设置存入金币金额 (1%)"))
        {
            DuoLog.Information($"{RetainerHandlers.SetDepositGilAmount(1)}");
        }
        if(ImGui.Button($"设置存入金币金额 (50%)"))
        {
            DuoLog.Information($"{RetainerHandlers.SetDepositGilAmount(50)}");
        }
        if(ImGui.Button($"设置存入金币金额 (99%)"))
        {
            DuoLog.Information($"{RetainerHandlers.SetDepositGilAmount(99)}");
        }
        if(ImGui.Button($"设置存入金币金额 (100%)"))
        {
            DuoLog.Information($"{RetainerHandlers.SetDepositGilAmount(100)}");
        }

        ImGui.Separator();

        if(ImGui.Button($"任务分配快速探险"))
        {
            TaskAssignQuickVenture.Enqueue();
        }
        if(ImGui.Button($"任务重新分配探险"))
        {
            TaskReassignVenture.Enqueue();
        }
        if(ImGui.Button($"任务提取金币 (50%)"))
        {
            TaskWithdrawGil.Enqueue(50);
        }

        ImGuiEx.Text($"空闲背包空格: {Utils.GetInventoryFreeSlotCount()}");
        ImGui.InputText("雇员名称", ref dbgRetName, 50);
        if(ImGui.Button("按名称选择雇员"))
        {
            DuoLog.Information($"{RetainerListHandlers.SelectRetainerByName(dbgRetName)}");
        }

        if(ImGui.Button("AtkStage 获取焦点"))
        {
            var ptr = (nint)AtkStage.Instance()->GetFocus();
            Svc.Chat.Print($"Stage focus: {ptr}");
        }
        if(ImGui.Button("AtkStage 清除焦点"))
        {
            AtkStage.Instance()->ClearFocus();
        }
        if(ImGui.Button("尝试获取当前雇员名称"))
        {
            if(TryGetAddonByName<AddonSelectString>("选择字符串", out var select) && IsAddonReady(&select->AtkUnitBase))
            {
                var textNode = (AtkTextNode*)select->AtkUnitBase.UldManager.NodeList[3];
                var text = GenericHelpers.ReadSeString(&textNode->NodeText);
                foreach(var x in text.Payloads)
                {
                    PluginLog.Information($"{x.Type}: {x.ToString()}");
                }
            }
        }
        {
            if(ImGui.Button("尝试关闭") && TryGetAddonByName<AtkUnitBase>("雇员列表", out var addon))
            {
                var v = stackalloc AtkValue[1]
                {
                                        new()
                                        {
                                                Type = AtkValueType.Int,
                                                Int = -1
                                        }
                                };
                addon->FireCallback(1, v);
                Notify.Info("已完成");
            }
        }
        {
            if(TryGetAddonByName<AtkUnitBase>("银行", out var addon) && IsAddonReady(addon))
            {
                if(ImGui.Button("测试银行"))
                {
                    var values = stackalloc AtkValue[2]
                    {
                                                new() { Type = AtkValueType.Int, Int = 3 },
                                                new() { Type = AtkValueType.UInt, Int = 50 },
                                        };
                    addon->FireCallback(2, values);
                }
            }
        }

        ImGui.Separator();

        if(ImGui.Button("任务分解物品"))
            TaskDesynthItems.Enqueue();
    }
}
