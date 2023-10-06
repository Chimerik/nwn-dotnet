using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnAcquireUpdateEncumbrance(ModuleEvents.OnAcquireItem onAcquireItem)
    {
      /*NwItem oItem = onAcquireItem.Item;

      if (onAcquireItem.AcquiredBy is null || onAcquireItem.AcquiredBy is not NwCreature oPC || oPC.ControllingPlayer is null
        || oItem is null)
        return;

      En pause jusqu'à ce que le système de transport soit en place
      if (oPC.MovementRate != MovementRate.Immobile && oPC.TotalWeight > Encumbrance2da.encumbranceTable.GetDataEntry(oPC.GetAbilityScore(Ability.Strength)).heavy)
      oPC.MovementRate = MovementRate.Immobile; */      
    }
    public static void OnUnAcquireUpdateEncumbrance(ModuleEvents.OnUnacquireItem onUncquireItem)
    {
      /*NwItem oItem = onAcquireItem.Item;

      if (onAcquireItem.AcquiredBy is null || onAcquireItem.AcquiredBy is not NwCreature oPC || oPC.ControllingPlayer is null
        || oItem is null)
        return;

      //En pause jusqu'à ce que le système de transport soit en place
      //if (onUnacquireItem.LostBy.MovementRate == MovementRate.Immobile)
      //if (onUnacquireItem.LostBy.TotalWeight <= Encumbrance2da.encumbranceTable.GetDataEntry(onUnacquireItem.LostBy.GetAbilityScore(Ability.Strength)).heavy)
      //onUnacquireItem.LostBy.MovementRate = MovementRate.PC;*/
    }
  }
}
