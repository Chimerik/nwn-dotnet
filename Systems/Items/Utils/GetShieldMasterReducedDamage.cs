using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class ItemUtils
  {
    public static int GetShieldMasterReducedDamage(NwCreature target, int damage, SavingThrowResult saveResult, Ability saveType = Ability.Dexterity)
    {
      if (damage > 0 && target is not null && saveType == Ability.Dexterity && target.KnowsFeat((Feat)CustomSkill.MaitreBouclier))
      {
        var reaction = target.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.ReactionEffectTag);

        if (reaction is not null)
        {

          bool saveFailed = saveResult == SavingThrowResult.Failure;

          switch (target.GetItemInSlot(InventorySlot.LeftHand)?.BaseItem.ItemType)
          {
            case BaseItemType.SmallShield:
            case BaseItemType.LargeShield:
            case BaseItemType.TowerShield:

              target.RemoveEffect(reaction);

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
      }

      return damage;
    }
  }
}
