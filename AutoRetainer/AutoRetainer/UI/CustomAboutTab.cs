using System.Diagnostics;

namespace AutoRetainer.UI
{
    public static class CustomAboutTab
    {
        private static string GetImageURL()
        {
            return Svc.PluginInterface.Manifest.IconUrl ?? "";
        }

        public static void Draw()
        {
            ImGuiEx.LineCentered("关于（一）", delegate
            {
                ImGuiEx.Text($"{Svc.PluginInterface.Manifest.Name} - {Svc.PluginInterface.Manifest.AssemblyVersion}");
            });

            ImGuiEx.LineCentered("关于（零）", () =>
            {
                ImGuiEx.Text($"由以下平台发布和开发：");
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.SameLine(0, 0);
                ImGuiEx.Text(ImGuiColors.DalamudRed, FontAwesomeIcon.Heart.ToIconString());
                ImGui.PopFont();
                ImGui.SameLine(0, 0);
                ImGuiEx.Text($"由 Puni.sh 和 NightmareXIV");
            });

            ImGuiHelpers.ScaledDummy(10f);
            ImGuiEx.LineCentered("关于（二）", delegate
            {
                if(ThreadLoadImageHandler.TryGetTextureWrap(GetImageURL(), out var texture))
                {
                    ImGui.Image(texture.Handle, new(200f, 200f));
                }
            });
            ImGuiHelpers.ScaledDummy(10f);
            ImGuiEx.LineCentered("关于（三）", delegate
            {
                ImGui.TextWrapped("加入我们的 Discord 社区以获取项目公告、更新和支持。");
            });
            ImGuiEx.LineCentered("关于（四）", delegate
            {
                if(ImGui.Button("Discord"))
                {
                    Process.Start(new ProcessStartInfo()
                    {
                        FileName = "https://discord.gg/Zzrcc8kmvy",
                        UseShellExecute = true
                    });
                }
                ImGui.SameLine();
                if(ImGui.Button("仓库"))
                {
                    ImGui.SetClipboardText("https://love.puni.sh/ment.json");
                    Notify.Success("链接已复制到剪贴板");
                }
                ImGui.SameLine();
                if(ImGui.Button("源代码"))
                {
                    Process.Start(new ProcessStartInfo()
                    {
                        FileName = Svc.PluginInterface.Manifest.RepoUrl,
                        UseShellExecute = true
                    });
                }
                ImGui.SameLine();
                if(ImGui.Button("捐赠给 Puni.sh 平台"))
                {
                    Process.Start(new ProcessStartInfo()
                    {
                        FileName = "https://ko-fi.com/spetsnaz",
                        UseShellExecute = true
                    });
                }
            });
        }
    }
}
