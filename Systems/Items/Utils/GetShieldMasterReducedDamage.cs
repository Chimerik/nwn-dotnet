using Anvil.API;

namespace NWN.Systems
{
  public static partial class ItemUtils
  {
    public static int GetShieldMasterReducedDamage(NwGameObject target, int damage, bool saveFailed, Ability saveType = Ability.Dexterity)
    {
      if (saveType == Ability.Dexterity && target is NwCreature creature
        && creature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.MaitreBouclier))
        && creature.GetObjectVariable<LocalVariableInt>(CreatureUtils.ShieldMasterCooldownVariable).HasNothing)
      {
        switch (creature.GetItemInSlot(InventorySlot.LeftHand)?.BaseItem.ItemType)
        {
          case BaseItemType.SmallShield:
          case BaseItemType.LargeShield:
          case BaseItemType.TowerShield:

            creature.GetObjectVariable<LocalVariableInt>(CreatureUtils.ShieldMasterCooldownVariable).Value = 1;
            ShieldMasterCooldown(creature);

            if (!saveFailed)
            {
              StringUtils.DisplayStringToAllPlayersNearTarget(creature, "Maître des boucliers", StringUtils.gold, true);
              LogUtils.LogMessage($"Maître des boucliers JDS réussi : Dégâts réduits à 0", LogUtils.LogType.Combat);
              return 0;
            }
            else
            {
              LogUtils.LogMessage($"Maître des boucliers JDS échoué : Dégâts {damage}", LogUtils.LogType.Combat);
              return damage / 2;
            }
        }
      }

      return damage;
    }
    private static async void ShieldMasterCooldown(NwCreature creature)
    {
      await NwTask.Delay(NwTimeSpan.FromRounds(1));
      creature.GetObjectVariable<LocalVariableInt>(CreatureUtils.ShieldMasterCooldownVariable).Delete();
    }
  }
}
