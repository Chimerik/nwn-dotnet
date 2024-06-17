using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int HandleSpellEvasion(NwCreature target, int damage, Ability saveAbility, bool saveFailed, int spellId = -1)
    {
      if (damage < 1)
        return damage;

      if (saveAbility == Ability.Dexterity)
      {
        if (target.KnowsFeat(Feat.ImprovedEvasion))
        {
          if (saveFailed)
          {
            damage /= 2;
            StringUtils.DisplayStringToAllPlayersNearTarget(target, "Esquive Totale", StringUtils.gold, true);
            LogUtils.LogMessage($"Esquive Totale JDS échoué : Dégâts {damage}", LogUtils.LogType.Combat);
          }
          else
          {
            damage = 0;
            StringUtils.DisplayStringToAllPlayersNearTarget(target, "Esquive Totale", StringUtils.gold, true);
            LogUtils.LogMessage($"Esquive Totale JDS réussi : Dégâts {damage}", LogUtils.LogType.Combat);
          }
        }
        else if (!saveFailed)
        {
          damage /= 2;
          LogUtils.LogMessage($"JDS réussi : Dégâts {damage}", LogUtils.LogType.Combat);
        }
      }
      else if(!saveFailed && Utils.In(spellId, CustomSpell.MauvaisAugure, CustomSpell.RayonDeLune))
      {
        damage /= 2;
        LogUtils.LogMessage($"JDS réussi : Dégâts {damage}", LogUtils.LogType.Combat);
      }

      return damage;
    }
  }
}
