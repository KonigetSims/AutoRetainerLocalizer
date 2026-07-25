namespace AutoRetainer.UI.Windows;
public class SingletonNotifyWindow : NotifyWindow
{
    private bool IAmIdiot = false;
    private WindowSystem ws;
    public SingletonNotifyWindow() : base("AutoRetainer - warning!")
    {
        IsOpen = true;
        ws = new();
        Svc.PluginInterface.UiBuilder.Draw += ws.Draw;
        ws.AddWindow(this);
    }

    public override void OnClose()
    {
        Svc.PluginInterface.UiBuilder.Draw -= ws.Draw;
    }

    public override void DrawContent()
    {
        ImGuiEx.Text($"AutoRetainer 检测到另一个插件实例正在运行，且使用了相同的数据路径配置。");
        ImGuiEx.Text($"为防止数据丢失，插件加载已中止。");
        if(ImGui.Button("关闭此窗口且不加载 AutoRetainer"))
        {
            IsOpen = false;
        }
        if(ImGui.Button("了解如何正确运行2个或更多游戏"))
        {
            ShellStart("https://github.com/PunishXIV/AutoRetainer/issues/62");
        }
        ImGui.Separator();
        ImGui.Checkbox($"勾选代表您同意可能会丢失所有 AutoRetainer 数据", ref IAmIdiot);
        if(!IAmIdiot) ImGui.BeginDisabled();
        if(ImGui.Button("加载 AutoRetainer"))
        {
            IsOpen = false;
            new TickScheduler(P.Load);
        }
        if(!IAmIdiot) ImGui.EndDisabled();
    }
}
