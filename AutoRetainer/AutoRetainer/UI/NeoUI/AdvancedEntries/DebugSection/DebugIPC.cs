
using ECommons.GameHelpers;
using ECommons.UIHelpers.AddonMasterImplementations;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace AutoRetainer.UI.NeoUI.AdvancedEntries.DebugSection;

internal unsafe class DebugIPC : DebugSectionBase
{
    public override void Draw()
    {
        ImGuiEx.Text($"获取最接近的雇员探险剩余秒数 {S.EzIPCManager.IPC_PluginState.GetClosestRetainerVentureSecondsRemaining(Player.CID)}");
        ImGui.Checkbox($"API 测试", ref ApiTest.Enabled);
        ImGuiEx.Text($"IPC 已抑制: {Svc.PluginInterface.GetIpcSubscriber<bool>("AutoRetainer.GetSuppressed").InvokeFunc()}");
        if(ImGui.Button($"抑制 = 是"))
        {
            Svc.PluginInterface.GetIpcSubscriber<bool, object>("AutoRetainer.SetSuppressed").InvokeAction(true);
        }
        if(ImGui.Button($"抑制 = 否"))
        {
            Svc.PluginInterface.GetIpcSubscriber<bool, object>("AutoRetainer.SetSuppressed").InvokeAction(false);
        }
        if(TryGetAddonByName<AddonSelectString>("选择字符串", out var sel))
        {
            var entries = Utils.GetEntries(sel);
            foreach(var x in entries)
            {
                var index = entries.IndexOf(x);
                if(ImGui.SmallButton($"{x} / {index}") && index >= 0)
                {
                    new AddonMaster.SelectString(sel).Entries[index].Select();
                }
            }
        }
    }
}
