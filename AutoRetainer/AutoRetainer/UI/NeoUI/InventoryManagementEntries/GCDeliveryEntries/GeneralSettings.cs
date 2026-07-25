using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoRetainer.UI.NeoUI.InventoryManagementEntries.GCDeliveryEntries;
public sealed unsafe class GeneralSettings : InventoryManagementBase
{
    public override string Name { get; } = "军队票券 - 一般设置";

    public override NuiBuilder Builder => new NuiBuilder()
        .Section("一般设置")
        .Checkbox("启用自动筹备交换", () => ref C.AutoGCContinuation)
        .TextWrapped($"""
            启用自动筹备交换后:
            - 插件会自动使用军票交换已设置的兑换列表中的物品。
            - 若列表为空，则只会交换探险币。
            - 请确认在角色设置中，"交付模式"未设为"停用"(Disabled)。
            
            军票兑换完成后:
            - 将继续执行筹备稀有品。
            - 该流程将重复至没有可筹备的物品或是军票使用完毕。
            """)

        .Section("多角色模式筹备交换")
        .TextWrapped($"""
        启用后:
        - 在多角色模式下，启用传送的角色会自动进行专家委托并根据兑换方案购买物品（前提是角色军衔足够）。
        """)
        .Checkbox("启用多角色筹备交换", () => ref C.FullAutoGCDelivery)
        .Checkbox("仅在工作台未锁定时触发", () => ref C.FullAutoGCDeliveryOnlyWsUnlocked)
        .InputInt(150f, "触发筹备的剩余背包格数 (小于或等于)", () => ref C.FullAutoGCDeliveryInventory, "仅计算主要背包，不包含装备库")
        .Checkbox("当探险币耗尽时触发", () => ref C.FullAutoGCDeliveryDeliverOnVentureExhaust, "此选项可能导致每次登录时都会前往军队兑换。请确保已设置足够探险币的方案。")
        .Indent()
        .InputInt(150f, "触发筹备的剩余探险币数量 (小于或等于)", () => ref C.FullAutoGCDeliveryDeliverOnVentureLessThan)
        .Unindent()
        .Checkbox("优先使用军票加成票券，如果可用的话", () => ref C.FullAutoGCDeliveryUseBuffItem)
        .Checkbox("优先使用部队军票加成BUFF，如果可用的话", () => ref C.FullAutoGCDeliveryUseBuffFCAction)
        .Checkbox("筹备交换后传送回房屋/旅馆", () => ref C.TeleportAfterGCExchange)
        .Indent()
        .Checkbox("仅在多角色模式启动时", () => ref C.TeleportAfterGCExchangeMulti)
        .Unindent()
        ;
}