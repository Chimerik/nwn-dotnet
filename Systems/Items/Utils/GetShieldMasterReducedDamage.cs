using Anvil.API;

namespace NWN.Systems
{
  public static partial class ItemUtils
  {
    public static int GetShieldMasterReducedDamage(NwCreature target, int damage, bool saveFailed, Ability saveType = Ability.Dexterity)
    {
      if (damage > 0 && saveType == Ability.Dexterity && target.KnowsFeat((Feat)CustomSkill.MaitreBouclier)
        && target.GetObjectVariable<LocalVariableInt>(CreatureUtils.ReactionVariable).Value > 0)
      {
        switch (target.GetItemInSlot(InventorySlot.LeftHand)?.BaseItem.ItemType)
        {
          case BaseItemType.SmallShield:
          case BaseItemType.LargeShield:
          case BaseItemType.TowerShield:

            target.GetObjectVariable<LocalVariableInt>(CreatureUtils.ReactionVariable).Value -= 1;

            if (!saveFailed)
            {
              StringUtils.DisplayStringToAllPlayersNearTarget(target, "Maître des boucliers", StringUtils.gold, true);
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
