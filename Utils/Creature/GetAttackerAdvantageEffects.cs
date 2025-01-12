using Anvil.API;
using NWN.Native.API;
using Ability = Anvil.API.Ability;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetAttackerAdvantageEffects(CNWSCreature attacker, CNWSCreature target, Ability attackStat, bool rangedAttack, NwSpell spell = null)
    {
      foreach (var eff in attacker.m_appliedEffects)
      {
        string tag = eff.m_sCustomTag.ToString();

        switch(tag)
        {
          case EffectSystem.ViseeStableEffectTag: GetViseeStableAdvantage(eff, attacker); return true; 
          case EffectSystem.AttaquesEtudieesEffectTag: GetAttaquesEtudieesAdvantage(eff, attacker); return true; 
          case EffectSystem.FouleeDombreEffectTag: LogUtils.LogMessage("Avantage - Foulée d'ombre", LogUtils.LogType.Combat); return true; 
          case EffectSystem.BroyeurEffectTag: LogUtils.LogMessage("Avantage - Broyeur", LogUtils.LogType.Combat); return true; 
          case EffectSystem.RecklessAttackEffectTag: LogUtils.LogMessage("Avantage - Frappe Téméraire", LogUtils.LogType.Combat); return true; 
          case EffectSystem.MaitreTactiqueTag: GetMaitreTactiqueAdvantage(eff, attacker); return true; 
          case EffectSystem.AssassinateEffectTag: LogUtils.LogMessage("Avantage - Assassinat", LogUtils.LogType.Combat); return true; 
          case EffectSystem.SensDivinEffectTag: if(GetSensDivinAdvantage(target)) return true; break; 
          case EffectSystem.SorcellerieInneeEffectTag: if(GetSorcellerieInneeAdvantage(spell)) return true; break; 
          case EffectSystem.VolEffectTag: if(GetVolRangedAdvantage(target, rangedAttack)) return true; break; 
        }
      }

      return false;
    }
  }
}
