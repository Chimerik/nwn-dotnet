using Anvil.API;

namespace NWN.Systems
{
  public static partial class ItemUtils
  {
    public static int GetShieldMasterBonusSave(NwCreature target, Ability saveType)
    {
      int shieldBonus = 0;

      if (saveType == Ability.Dexterity && target.KnowsFeat(NwFeat.FromFeatId(CustomSkill.MaitreBouclier)))
      {
        shieldBonus = (target.GetItemInSlot(InventorySlot.LeftHand)?.BaseItem.ItemType) switch
        {
          BaseItemType.SmallShield => 1,
          BaseItemType.LargeShield => 2,
          BaseItemType.TowerShield => 3,
          _ => 0,
        };

        LogUtils.LogMessage($"Maître des boucliers : JDS +{shieldBonus}", LogUtils.LogType.Combat);
      }

      return shieldBonus;
    }
  }
}
