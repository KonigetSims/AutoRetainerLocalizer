using AutoRetainerAPI.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoRetainer.UI.NeoUI.AdvancedEntries.DebugSection;
public sealed unsafe class DebugNeoGCDelivery : DebugSectionBase
{
    public override void Draw()
    {
        if(ImGui.Button("开始新购买")) GCContinuation.BeginNewPurchase();
        foreach(var x in Utils.SharedGCExchangeListings.Values)
        {
            ImGuiEx.Text($"{x.Data.Name} / {x.ItemID} / {x.Category} / 最低军衔 {x.MinPurchaseRank} {x.Rank} / {x.Seals} 军票 | 可购买: x{new GCExchangeItem(x.ItemID, int.MaxValue).GetAmountThatCanBePurchased()}");
        }
    }
}