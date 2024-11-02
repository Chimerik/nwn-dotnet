using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int HandleSpellEvasion(NwCreature target, int damage, Ability saveAbility, SavingThrowResult saveResult, int spellId = -1, byte spellLevel = 255)
    {
      if (damage < 1 || target is null)
        return damage;

      if (saveResult == SavingThrowResult.Immune)
      {
        LogUtils.LogMessage("Cible immunisée : Dégâts 0", LogUtils.LogType.Combat);
        return 0;
      }

      bool saveFailed = saveResult == SavingThrowResult.Failure;

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
      else if(!saveFailed 
        && (Utils.In(spellId, CustomSpell.MauvaisAugure, CustomSpell.RayonDeLune, CustomSpell.EspritsGardiensRadiant, CustomSpell.EspritsGardiensNecrotique,
        (int)Spell.Balagarnsironhorn, (int)Spell.Enervation, CustomSpell.Fracassement, CustomSpell.PoingDeLair, CustomSpell.Rupture, CustomSpell.VagueDestructriceNecrotique, CustomSpell.VagueDestructriceRadiant)
        || spellLevel == 0))
      {
        damage /= 2;
        LogUtils.LogMessage($"JDS réussi : Dégâts {damage}", LogUtils.LogType.Combat);
      }

      return damage;
    }
  }
}
