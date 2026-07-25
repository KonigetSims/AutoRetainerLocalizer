using System;
using System.Collections.Generic;
using System.Text;

namespace AutoRetainer.UI.NeoUI.MultiModeEntries;

public class MultiModeDisableRender : NeoUIEntry
{
    public override string Path => "多角色模式/禁用渲染";

    public override NuiBuilder Builder => new NuiBuilder()
        .Section("禁用渲染")
        .Checkbox("在多角色模式下禁用渲染", () => ref C.MultiDisableRender, "在多角色模式下禁用世界渲染。")
        .Checkbox("仅在夜间模式下", () => ref C.MultiDisableRenderNightModeOnly)
        .Checkbox("仅在窗口非活动时", () => ref C.MultiDisableRenderOnlyInactive);
}
