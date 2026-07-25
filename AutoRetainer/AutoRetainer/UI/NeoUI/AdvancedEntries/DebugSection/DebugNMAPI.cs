namespace AutoRetainer.UI.NeoUI.AdvancedEntries.DebugSection;

internal class DebugNMAPI : DebugSectionBase
{
    private static float vol;
    private static bool repeat;
    private static bool stopOnFocus;
    private static string path = "";
    public override void Draw()
    {
        ImGuiEx.Text($"激活: {P.NotificationMasterApi.IsIPCReady()}");
        ImGui.InputText("路径", ref path, 500);
        ImGui.InputFloat("vol", ref vol);
        ImGui.Checkbox("重复", ref repeat);
        ImGui.Checkbox("获得焦点时停止", ref stopOnFocus);
        if(ImGui.Button("闪烁")) new TickScheduler(() => P.NotificationMasterApi.FlashTaskbarIcon(), 1000);
        if(ImGui.Button("消息")) new TickScheduler(() => P.NotificationMasterApi.DisplayTrayNotification("标题", "文本"), 1000);
        if(ImGui.Button("无标题消息")) new TickScheduler(() => P.NotificationMasterApi.DisplayTrayNotification("文本"), 1000);
        if(ImGui.Button("播放声音")) new TickScheduler(() => P.NotificationMasterApi.PlaySound(path, vol, repeat, stopOnFocus), 1000);
        if(ImGui.Button("停止声音")) P.NotificationMasterApi.StopSound();
    }
}
