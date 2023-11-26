using Anvil.API;

namespace NWN.Systems
{
  public static partial class ItemUtils
  {
    public static int GetShieldMasterBonusSave(NwCreature target, Ability saveType)
    {
      if (saveType == Ability.Dexterity && target.KnowsFeat(NwFeat.FromFeatId(CustomSkill.MaitreBouclier)))
      {
        return (target.GetItemInSlot(InventorySlot.LeftHand)?.BaseItem.ItemType) switch
        {
          BaseItemType.SmallShield => 1,
          BaseItemType.LargeShield => 2,
          BaseItemType.TowerShield => 3,
          _ => 0,
        };
      }

      return 0;
    }
  }
}
