using AutoRetainer.Internal;
using AutoRetainer.Scheduler.Handlers;
using AutoRetainer.Scheduler.Tasks;
using AutoRetainerAPI;
using AutoRetainerAPI.Configuration;
using Dalamud.Interface.Components;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace AutoRetainer.UI.Overlays;

internal unsafe class RetainerListOverlay : Window
{
    private float height;
    internal volatile string PluginToProcess = null;

    public RetainerListOverlay() : base("AutoRetainer retainerlist overlay", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoFocusOnAppearing, true)
    {
        P.WindowSystem.AddWindow(this);
        RespectCloseHotkey = false;
        IsOpen = true;
    }

    public override bool DrawConditions()
    {
        if(!C.UIBar) return false;
        if(Svc.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.OccupiedSummoningBell] && TryGetAddonByName<AtkUnitBase>("雇员列表", out var addon) && IsAddonReady(addon))
        {
            Position = new(addon->X, addon->Y - height);
            return true;
        }
        return false;
    }

    public override void PreDraw()
    {
        //ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
    }

    public override void Draw()
    {
        var e = SchedulerMain.PluginEnabled;
        var disabled = MultiMode.Active && !ImGui.GetIO().KeyCtrl;
        if(disabled)
        {
            ImGui.BeginDisabled();
        }
        if(ImGui.Checkbox("启用 AutoRetainer", ref e))
        {
            P.WasEnabled = false;
            if(e)
            {
                SchedulerMain.EnablePlugin(PluginEnableReason.Manual);
            }
            else
            {
                SchedulerMain.DisablePlugin();
            }
        }
        if(disabled)
        {
            ImGui.EndDisabled();
            ImGuiComponents.HelpMarker($"多角色模式正控制此选项。按住 CTRL 可强制覆盖。");
        }
        if(P.WasEnabled)
        {
            ImGui.SameLine();
            ImGuiEx.Text(GradientColor.Get(ImGuiColors.DalamudGrey, ImGuiColors.DalamudGrey3, 500), $"已暂停");
        }
        if(C.MultiModeUIBar)
        {
            ImGui.SameLine();
            if(ImGui.Checkbox("多角色模式", ref MultiMode.Enabled))
            {
                MultiMode.OnMultiModeEnabled();
                if(MultiMode.Active)
                {
                    SchedulerMain.EnablePlugin(PluginEnableReason.MultiMode);
                }
            }
        }

        Svc.PluginInterface.GetIpcProvider<object>(ApiConsts.OnMainControlsDraw).SendMessage();

        ImGui.SameLine();

        if(ImGuiEx.IconButton($"{Lang.IconSettings}##打开插件界面"))
        {
            Svc.Commands.ProcessCommand("/ays");
        }
        ImGuiEx.Tooltip("打开插件设置");
        if(!P.TaskManager.IsBusy)
        {
            ImGui.SameLine();
            if(ImGuiEx.IconButton($"{Lang.IconDuplicate}##存放所有重复物品"))
            {
                for(var i = 0; i < GameRetainerManager.Count; i++)
                {
                    var ret = GameRetainerManager.Retainers[i];
                    if(ret.Available)
                    {
                        var adata = Utils.GetAdditionalData(Data.CID, ret.Name);
                        var selectedPlan = C.EntrustPlans.FirstOrDefault(x => x.Guid == adata.EntrustPlan);
                        if(selectedPlan != null)
                        {
                            P.TaskManager.Enqueue(() => RetainerListHandlers.SelectRetainerByName(ret.Name.ToString()));
                            TaskEntrustDuplicates.EnqueueNew(selectedPlan);
                            if(C.RetainerMenuDelay > 0)
                            {
                                TaskWaitSelectString.Enqueue(C.RetainerMenuDelay);
                            }
                            P.TaskManager.Enqueue(RetainerHandlers.SelectQuit);
                        }
                        else
                        {
                            Notify.Error($"找不到雇员 {ret.Name} 的存放计划");
                        }

                    }
                }
            }
            ImGuiEx.Tooltip("快速存放");

            ImGui.SameLine();
            if(ImGuiEx.IconButton($"{FontAwesomeIcon.ArrowRightToBracket.ToIconString()}##手动委托"))
            {
                ImGui.OpenPopup("EntrustManually");
            }
            if(ImGui.BeginPopup("EntrustManually"))
            {
                foreach(var selectedPlan in C.EntrustPlans)
                {
                    ImGui.PushID(selectedPlan.Guid.ToString());
                    if(ImGui.Selectable($"{selectedPlan.Name}"))
                    {
                        for(var i = 0; i < GameRetainerManager.Count; i++)
                        {
                            var ret = GameRetainerManager.Retainers[i];
                            if(ret.Available)
                            {
                                var adata = Utils.GetAdditionalData(Data.CID, ret.Name);
                                if(selectedPlan != null)
                                {
                                    P.TaskManager.Enqueue(() => RetainerListHandlers.SelectRetainerByName(ret.Name.ToString()));
                                    TaskEntrustDuplicates.EnqueueNew(selectedPlan);
                                    if(C.RetainerMenuDelay > 0)
                                    {
                                        TaskWaitSelectString.Enqueue(C.RetainerMenuDelay);
                                    }
                                    P.TaskManager.Enqueue(RetainerHandlers.SelectQuit);
                                }
                            }
                        }
                    }
                    ImGui.PopID();
                }
                ImGui.EndPopup();
            }
            ImGuiEx.Tooltip("运行特定的委托计划");

            ImGui.SameLine();
            if(ImGuiEx.IconButton($"{FontAwesomeIcon.ArrowRightFromBracket.ToIconString()}##反向委托"))
            {
                ImGui.OpenPopup("ReverseEntrust");
            }
            if(ImGui.BeginPopup("ReverseEntrust")) 
            { 
                foreach(var selectedPlan in C.EntrustPlans)
                {
                    ImGui.PushID(selectedPlan.Guid.ToString());
                    if(ImGui.Selectable($"{selectedPlan.Name}"))
                    {
                        for(var i = 0; i < GameRetainerManager.Count; i++)
                        {
                            var ret = GameRetainerManager.Retainers[i];
                            if(ret.Available)
                            {
                                var adata = Utils.GetAdditionalData(Data.CID, ret.Name);
                                if(selectedPlan != null)
                                {
                                    P.TaskManager.Enqueue(() => RetainerListHandlers.SelectRetainerByName(ret.Name.ToString()));
                                    TaskEntrustDuplicates.EnqueueNewReverse(selectedPlan);
                                    if(C.RetainerMenuDelay > 0)
                                    {
                                        TaskWaitSelectString.Enqueue(C.RetainerMenuDelay);
                                    }
                                    P.TaskManager.Enqueue(RetainerHandlers.SelectQuit);
                                }
                            }
                        }
                    }
                    ImGui.PopID();
                }
                ImGui.EndPopup();
            }
            ImGuiEx.Tooltip("反向运行特定的委托计划（根据委托计划从雇员处提取物品）");

            ImGui.SameLine();
            if(ImGuiEx.IconButton($"{Lang.IconGil}##提取金币"))
            {
                for(var i = 0; i < GameRetainerManager.Count; i++)
                {
                    var ret = GameRetainerManager.Retainers[i];
                    if(ret.Available)
                    {
                        P.TaskManager.Enqueue(() => RetainerListHandlers.SelectRetainerByName(ret.Name.ToString()));
                        TaskWithdrawGil.Enqueue(100);

                        if(C.RetainerMenuDelay > 0)
                        {
                            TaskWaitSelectString.Enqueue(C.RetainerMenuDelay);
                        }
                        P.TaskManager.Enqueue(RetainerHandlers.SelectQuit);
                    }
                }
            }
            ImGuiEx.Tooltip("快速提取金币");

            {
                ImGui.SameLine();
                if(ImGuiEx.IconButton($"{Lang.IconFire}##出售物品"))
                {
                    Utils.EnqueueVendorItemsByRetainer();
                }
                if(ImGui.IsItemClicked(ImGuiMouseButton.Right))
                {
                    ImGui.OpenPopup("QuickVendorPopup");
                }
                ImGuiEx.Tooltip("快速出售物品");
                if(ImGui.BeginPopup("QuickVendorPopup"))
                {
                    if(ImGui.Selectable("从筹备物资列表中出售物品"))
                    {
                        for(var i = 0; i < GameRetainerManager.Count; i++)
                        {
                            var ret = GameRetainerManager.Retainers[i];
                            if(ret.Available)
                            {
                                P.TaskManager.Enqueue(() => RetainerListHandlers.SelectRetainerByName(ret.Name.ToString()));
                                TaskVendorItems.Enqueue(true);

                                if(C.RetainerMenuDelay > 0)
                                {
                                    TaskWaitSelectString.Enqueue(C.RetainerMenuDelay);
                                }
                                P.TaskManager.Enqueue(RetainerHandlers.SelectQuit);
                                P.TaskManager.Enqueue(RetainerHandlers.ConfirmCantBuyback);
                                break;
                            }
                        }
                    }
                    ImGui.EndPopup();
                }
            }

            PluginToProcess = null;
            Svc.PluginInterface.GetIpcProvider<object>(ApiConsts.OnRetainerListTaskButtonsDraw).SendMessage();
            if(PluginToProcess != null)
            {
                for(var i = 0; i < GameRetainerManager.Count; i++)
                {
                    var ret = GameRetainerManager.Retainers[i];
                    if(ret.Available)
                    {
                        P.TaskManager.Enqueue(() => RetainerListHandlers.SelectRetainerByName(ret.Name.ToString()));
                        TaskPostprocessRetainerIPC.Enqueue(ret.Name.ToString(), PluginToProcess);

                        if(C.RetainerMenuDelay > 0)
                        {
                            TaskWaitSelectString.Enqueue(C.RetainerMenuDelay);
                        }
                        P.TaskManager.Enqueue(RetainerHandlers.SelectQuit);
                        P.TaskManager.Enqueue(RetainerHandlers.ConfirmCantBuyback);
                    }
                }
            }
        }
        height = ImGui.GetWindowSize().Y;
    }

    public override void PostDraw()
    {
        //ImGui.PopStyleVar();
    }
}
