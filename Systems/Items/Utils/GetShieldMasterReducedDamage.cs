using Anvil.API;

namespace NWN.Systems
{
  public static partial class ItemUtils
  {
    public static int GetShieldMasterReducedDamage(NwGameObject target, int damage, bool saveFailed, Ability saveType = Ability.Dexterity)
    {
      if (saveType == Ability.Dexterity && target is NwCreature creature
        && creature.KnowsFeat((Feat)CustomSkill.MaitreBouclier)
        && creature.GetObjectVariable<LocalVariableInt>(CreatureUtils.ReactionVariable).Value > 0)
      {
        switch (creature.GetItemInSlot(InventorySlot.LeftHand)?.BaseItem.ItemType)
        {
          case BaseItemType.SmallShield:
          case BaseItemType.LargeShield:
          case BaseItemType.TowerShield:

            creature.GetObjectVariable<LocalVariableInt>(CreatureUtils.ReactionVariable).Value -= 1;

            if (!saveFailed)
            {
              StringUtils.DisplayStringToAllPlayersNearTarget(creature, "Maître des boucliers", StringUtils.gold, true);
              LogUtils.LogMessage($"Maître des boucliers JDS réussi : Dégâts réduits à 0", LogUtils.LogType.Combat);
              return 0;
            }
            else
            {
              damage /= 2;
              LogUtils.LogMessage($"Maître des boucliers JDS échoué : Dégâts {damage}", LogUtils.LogType.Combat);
            }

            break;
        }
      }

      return damage;
    }
  }
}
