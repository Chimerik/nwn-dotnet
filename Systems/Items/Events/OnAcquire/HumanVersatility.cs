using Anvil.API;
using System;
using static Anvil.API.Events.ModuleEvents;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnAcquireCheckHumanVersatility(OnAcquireItem onAcquireItem)
    {
      NwItem item = onAcquireItem.Item;

      if (item is null || item.Weight == 0)
        return;

      item.GetObjectVariable<LocalVariableFloat>("_ITEM_WEIGHT_PRE_VERSATILITY").Value = (float)item.Weight / item.StackSize;
      ModuleSystem.Log.Info($"weight before : {item.Weight}");
      decimal newWeigth = item.Weight / item.StackSize * (decimal)0.75;

      if (newWeigth < (decimal)0.1)
        newWeigth = (decimal)0.1;

      item.Weight = newWeigth;
      ModuleSystem.Log.Info($"weight after : {item.Weight}");
    }
    public static void OnUnAcquireCheckHumanVersatility(OnUnacquireItem onUnacqItem)
    {
      NwItem item = onUnacqItem.Item;

      if (item is null || item.GetObjectVariable<LocalVariableString>("_ITEM_WEIGHT_PRE_VERSATILITY").HasNothing)
        return;

      ModuleSystem.Log.Info($"weight variable : {item.GetObjectVariable<LocalVariableFloat>("_ITEM_WEIGHT_PRE_VERSATILITY").Value}");
      ModuleSystem.Log.Info($"weight variable converted: {(decimal)item.GetObjectVariable<LocalVariableFloat>("_ITEM_WEIGHT_PRE_VERSATILITY").Value}");
      item.Weight = (decimal)item.GetObjectVariable<LocalVariableFloat>("_ITEM_WEIGHT_PRE_VERSATILITY").Value;
      item.GetObjectVariable<LocalVariableFloat>("_ITEM_WEIGHT_PRE_VERSATILITY").Delete();
      ModuleSystem.Log.Info($"weight after : {item.Weight}");
    }
  }
}
