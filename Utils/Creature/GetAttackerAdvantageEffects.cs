using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetAttackerAdvantageEffects(Native.API.CNWSCreature attacker, Native.API.CNWSCreature target, Ability attackStat, bool rangedAttack, NwSpell spell = null)
    {
      foreach (var eff in attacker.m_appliedEffects)
      {
        if (GetTrueStrikeAdvantage(eff))
          return true;

        if (GetViseeStableAdvantage(eff, attacker))
          return true;

        if (GetAttaquesEtudieesAdvantage(eff, attacker))
          return true;

        if (GetFouleeDombreAdvantage(eff))
          return true;

        if (GetBroyeurAdvantage(eff))
          return true;

        if (GetRecklessAttackAdvantage(eff))
          return true;

        if (GetMaitreTactiqueAdvantage(eff))
        {
          DelayEffectRemoval(attacker);
          return true;
        }

        if (GetAssassinateAdvantage(eff))
          return true;

        if (GetSensDivinAdvantage(eff, target))
          return true;

        if (GetSorcellerieInneeAdvantage(eff, spell))
          return true;

        if(GetVolRangedAdvantage(eff, attacker, target, rangedAttack))
          return true;
      }

      return false;
    }
    private static async void DelayEffectRemoval(Native.API.CNWSCreature attacker)
    {
      await NwTask.NextFrame();
      foreach (var eff in attacker.m_appliedEffects)
        if (eff.m_sCustomTag.CompareNoCase(EffectSystem.MaitreTactiqueExoTag).ToBool())
          attacker.RemoveEffect(eff);
    }
  }
}
