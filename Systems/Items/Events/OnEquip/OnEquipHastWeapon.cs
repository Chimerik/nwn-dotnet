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

      if (oCreature is null || oItem is null)
        return;

      switch(oItem.BaseItem.ItemType)
      {
        case BaseItemType.Halberd:
        case BaseItemType.Whip:
        case BaseItemType.ShortSpear:

          if (oCreature.GetObjectVariable<LocalVariableInt>("_HAST_WEAPON_EQUIPPED").HasNothing)
          {
            CreaturePlugin.SetHitDistance(oCreature, CreaturePlugin.GetHitDistance(oCreature) * 2);
            oCreature.GetObjectVariable<LocalVariableInt>("_HAST_WEAPON_EQUIPPED").Value = 1;

            EffectUtils.RemoveTaggedEffect(oCreature, oCreature, EffectSystem.ThreatenedAoETag);
            CreatureUtils.InitThreatRange(oCreature);
          }

          break;

        default:

          if (oCreature.GetObjectVariable<LocalVariableInt>("_HAST_WEAPON_EQUIPPED").HasValue)
          {
            CreaturePlugin.SetHitDistance(oCreature, CreaturePlugin.GetHitDistance(oCreature) / 2);
            oCreature.GetObjectVariable<LocalVariableInt>("_HAST_WEAPON_EQUIPPED").Delete();

            EffectUtils.RemoveTaggedEffect(oCreature, oCreature, EffectSystem.ThreatenedAoETag);
            CreatureUtils.InitThreatRange(oCreature);
          }

          break;
      }
    }
    public static void InitHastWeaponHitDistance(NwCreature creature)
    {
      NwItem oItem = creature.GetItemInSlot(InventorySlot.RightHand);

      if (oItem is null)
        return;

      switch (oItem.BaseItem.ItemType)
      {
        case BaseItemType.Halberd:
        case BaseItemType.Whip:
        case BaseItemType.ShortSpear:

          if (creature.GetObjectVariable<LocalVariableInt>("_HAST_WEAPON_EQUIPPED").HasNothing)
          {
            CreaturePlugin.SetHitDistance(creature, CreaturePlugin.GetHitDistance(creature) * 2);
            creature.GetObjectVariable<LocalVariableInt>("_HAST_WEAPON_EQUIPPED").Value = 1;

            EffectUtils.RemoveTaggedEffect(creature, creature, EffectSystem.ThreatenedAoETag);
            CreatureUtils.InitThreatRange(creature);
          }

          break;
      }
    }
  }
}
