using ECommons.Throttlers;

namespace AutoRetainer.UI.NeoUI.MultiModeEntries;
public class MultiModeDeployables : NeoUIEntry
{
    public override string Path => "多角色模式/远航探索";

    public override NuiBuilder Builder { get; init; } = new NuiBuilder()
        .Section("多角色模式 - 潜艇/飞空艇")
        .Checkbox("等待航程完成", () => ref C.MultiModeWorkshopConfiguration.MultiWaitForAll, "启用时，AutoRetainer 会等到所有探险潜艇回归后才登录该角色。若你因其他原因已在线，它仍会重新派遣已完成的潜艇——除非\"即使已登录也等待\"的全局设置也被开启。")
        .Indent()
        .Checkbox("即使已登录也等待", () => ref C.MultiModeWorkshopConfiguration.WaitForAllLoggedIn, "更改\"等待航行完成\"的行为（包括全局与单一角色设置），使 AutoRetainer 在已登录时不再单独派遣个别回归的潜艇，而是等到\"全部\"潜艇都回归后才一并处理。")
        .InputInt(120f, "最大等待时间（分钟）", () => ref C.MultiModeWorkshopConfiguration.MaxMinutesOfWaiting.ValidateRange(0, 9999), 10, 60, "如果等待其余潜艇回归的时间超过此分钟数，AutoRetainer 将忽略\"等待航行完成\"与\"即使已登录也等待\"的设置。")
        .Unindent()
        .DragInt(60f, "Advance Relog Threshold, seconds", () => ref C.MultiModeWorkshopConfiguration.AdvanceTimer.ValidateRange(0, 300), 0.1f, 0, 300, "The number of seconds AutoRetainer should log in early before submarines on this character are ready to be resent.")
        .DragInt(120f, "Retainer venture processing cutoff, minutes", () => ref C.DisableRetainerVesselReturn.ValidateRange(0, 60), "If set to a value greater than 0, AutoRetainer will stop processing any retainers this number of minutes before any character is scheduled to redeploy submarines, taking all previous settings into account.")
        .Checkbox("派遣后立即出售\"无条件出售列表\"中的物品（需要雇员）", () => ref C.VendorItemAfterVoyage)
        .Checkbox("进入部队工坊时，定期检查部队箱中的金币", () => ref C.FCChestGilCheck, "在进入工坊时定期检查部队箱，以保持金币计数为最新状态。")
        .Indent()
        .SliderInt(150f, "Check frequency, hours", () => ref C.FCChestGilCheckCd, 0, 24 * 5)
        .Widget("重置冷却时间", (x) =>
        {
            if(ImGuiEx.Button(x, C.FCChestGilCheckTimes.Count > 0)) C.FCChestGilCheckTimes.Clear();
        })
        .Unindent()
        .Checkbox("处理完所有远航探索后关闭游戏", () => ref C.ShutdownOnSubExhaustion)
        .Indent()
        .SliderFloat(150f, "Don't shutdown if there are deployables that return within this amount of hours", () => ref C.HoursForShutdown, 0f, 10f)
        .Widget(() =>
        {
            ImGuiEx.HelpMarker($"""
                当前状态：{(Utils.CanShutdownForSubs() ? "Can shutdown" : "Can NOT shutdown")}\n距离强制关机剩余：{EzThrottler.GetRemainingTime("ForceShutdownForSubs")}
                """);
        })
        .Unindent()
        .TextWrapped("进入工坊后自动购买青磷水：")
        .Indent()
        .Widget(() =>
        {
            if(Data != null)
            {
                ImGui.Checkbox($"在 {Data.NameWithWorldCensored} 上启用", ref Data.AutoFuelPurchase);
            }
            ImGuiEx.TextWrapped($"若要启用/禁用其他角色的燃料购买，请前往「功能、排除与排序」区块。");
        })
        .InputInt(150f, "触发购买的剩余青磷水数量", () => ref C.AutoFuelPurchaseLow.ValidateRange(100, 99999))
        .InputInt(150f, "购买至背包内达到此数量", () => ref C.AutoFuelPurchaseMax)
        .Checkbox("仅在工作站解锁时进行购买", () => ref C.AutoFuelPurchaseOnlyWsUnlocked)
        .Unindent()
        .Checkbox("在远航探索完成后退出游戏", () => ref C.ExitOnSubCompletion, "重要提示：激活后，您的多角色模式将仅处理远航探索，不处理雇员。")
        .Indent()
        .InputInt(150f, "等待潜艇返回的最大时间（分钟）", () => ref C.ExitOnSubCompletionTime)
        .Unindent()
        ;
}
