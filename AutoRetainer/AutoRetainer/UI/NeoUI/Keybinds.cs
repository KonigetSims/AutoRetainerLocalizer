namespace AutoRetainer.UI.NeoUI;
public class Keybinds : NeoUIEntry
{
    public override string Path => "快捷键设置";

    public override NuiBuilder Builder { get; init; } = new NuiBuilder()
        .Section("访问召唤铃/控制面板的快捷键")
        .Widget("使用召唤铃/控制面板时，暂时防止 AutoRetainer 自动启动", (x) =>
        {
            UIUtils.DrawKeybind(x, ref C.Suppress);
        })
        .Widget("暂时设置为仅领取模式，防止在当前循环中分配任务/暂时将潜艇模式设置为仅结算", (x) =>
        {
            UIUtils.DrawKeybind(x, ref C.TempCollectB);
        })

        .Section("雇员快速动作")
        .Widget("出售物品", (x) => UIUtils.QRA(x, ref C.SellKey))
        .Widget("存放物品", (x) => UIUtils.QRA(x, ref C.EntrustKey))
        .Widget("取回物品", (x) => UIUtils.QRA(x, ref C.RetrieveKey))
        .Widget("上架出售", (x) => UIUtils.QRA(x, ref C.SellMarketKey));
}
