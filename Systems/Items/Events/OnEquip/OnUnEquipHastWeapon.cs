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

      if (oCreature is null || oItem is null)
        return;

      switch (oItem.BaseItem.ItemType)
      {
        case BaseItemType.Halberd:
        case BaseItemType.Whip:
        case BaseItemType.ShortSpear:

          CreaturePlugin.SetHitDistance(oCreature, CreaturePlugin.GetHitDistance(oCreature) / 2);
          oCreature.GetObjectVariable<LocalVariableInt>("_HAST_WEAPON_EQUIPPED").Delete();

          EffectUtils.RemoveTaggedEffect(oCreature, oCreature, EffectSystem.ThreatenedAoETag);
          CreatureUtils.InitThreatRange(oCreature);

          break;
      }
    }
  }
}
