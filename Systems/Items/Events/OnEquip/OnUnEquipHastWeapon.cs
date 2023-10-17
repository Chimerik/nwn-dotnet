using Anvil.API;
using Anvil.API.Events;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnUnequipHastWeapon(OnItemUnequip onUnequip)
    {
      NwCreature oCreature = onUnequip.Creature;
      NwItem oItem = onUnequip.Item;

      if (oCreature is null || oItem is null || oCreature.GetObjectVariable<LocalVariableInt>("_HAST_WEAPON_EQUIPPED").HasNothing)
        return;

      switch (oItem.BaseItem.ItemType)
      {
        case BaseItemType.Halberd:
        case BaseItemType.Greatsword:
        case BaseItemType.Whip:
        case BaseItemType.ShortSpear:

          CreaturePlugin.SetHitDistance(oCreature, CreaturePlugin.GetHitDistance(oCreature) / 2);
          oCreature.GetObjectVariable<LocalVariableInt>("_HAST_WEAPON_EQUIPPED").Delete();
          break;
      }
    }
  }
}
