using Anvil.API;

namespace NWN.Systems
{
  public static partial class ItemUtils
  {
    public static int GetShieldMasterReducedDamage(NwGameObject target, int damage, bool saveFailed, Ability saveType = Ability.Dexterity)
    {
      if (saveType == Ability.Dexterity && target is NwCreature creature
        && creature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.MaitreBouclier))
        && creature.GetObjectVariable<LocalVariableInt>(CreatureUtils.ReactionVariable).HasValue)
      {
        switch (creature.GetItemInSlot(InventorySlot.LeftHand)?.BaseItem.ItemType)
        {
          case BaseItemType.SmallShield:
          case BaseItemType.LargeShield:
          case BaseItemType.TowerShield:

            creature.GetObjectVariable<LocalVariableInt>(CreatureUtils.ReactionVariable).Delete();

            if (!saveFailed)
            {
              StringUtils.DisplayStringToAllPlayersNearTarget(creature, "Maître des boucliers", StringUtils.gold, true);
              LogUtils.LogMessage($"{target.Name} - Maître des boucliers - Dégâts parés", LogUtils.LogType.Combat);
              return 0;
            }
            else
            {
              LogUtils.LogMessage($"{target.Name} - Maître des boucliers - Dégâts à demi parés", LogUtils.LogType.Combat);
              return damage / 2;
            }
        }
      }

      return damage;
    }
  }
}
