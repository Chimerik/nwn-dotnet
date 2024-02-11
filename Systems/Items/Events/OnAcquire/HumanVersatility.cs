using Anvil.API;
using static Anvil.API.Events.ModuleEvents;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnAcquireCheckHumanVersatility(OnAcquireItem onAcquireItem)
    {
      NwItem item = onAcquireItem.Item;

      if (item is null || item.Weight == 0 || onAcquireItem.AcquiredBy is not NwCreature creature)
        return;

      item.GetObjectVariable<LocalVariableFloat>("_ITEM_WEIGHT_PRE_VERSATILITY").Value = (float)item.Weight / item.StackSize;
      decimal newWeigth = item.Weight / item.StackSize;

      if (creature.Race.RacialType == RacialType.Human)
        newWeigth *= (decimal)0.75;

      if(creature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.TotemAspectOurs)))
        newWeigth *= (decimal)0.5;

      if (newWeigth < (decimal)0.1)
        newWeigth = (decimal)0.1;

      item.Weight = newWeigth;
    }
    public static void OnUnAcquireCheckHumanVersatility(OnUnacquireItem onUnacqItem)
    {
      NwItem item = onUnacqItem.Item;

      if (item is null || item.GetObjectVariable<LocalVariableString>("_ITEM_WEIGHT_PRE_VERSATILITY").HasNothing)
        return;

      item.Weight = (decimal)item.GetObjectVariable<LocalVariableFloat>("_ITEM_WEIGHT_PRE_VERSATILITY").Value;
      item.GetObjectVariable<LocalVariableFloat>("_ITEM_WEIGHT_PRE_VERSATILITY").Delete();
    }
  }
}
