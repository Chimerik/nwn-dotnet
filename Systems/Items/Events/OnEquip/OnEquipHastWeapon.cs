using Anvil.API;
using Anvil.API.Events;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnEquipHastWeapon(OnItemEquip onEquip)
    {
      NwCreature oCreature = onEquip.EquippedBy;
      NwItem oItem = onEquip.Item;

      if (oCreature is null || oItem is null || oCreature.GetObjectVariable<LocalVariableInt>("_HAST_WEAPON_EQUIPPED").HasValue)
        return;

      switch(oItem.BaseItem.ItemType)
      {
        case BaseItemType.Halberd:
        case BaseItemType.Greatsword:
        case BaseItemType.Whip:
        case BaseItemType.ShortSpear:
          CreaturePlugin.SetHitDistance(oCreature, CreaturePlugin.GetHitDistance(oCreature) * 2);
          oCreature.GetObjectVariable<LocalVariableInt>("_HAST_WEAPON_EQUIPPED").Value = 1;
          break;
      }
    }
  }
}
