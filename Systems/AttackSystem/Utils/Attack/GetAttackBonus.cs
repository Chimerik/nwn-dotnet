using System.Linq;
using System.Numerics;
using Anvil.API;
using NWN.Core;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetAttackBonus(CNWSCreature attacker, CNWSCreature target, CNWSCombatAttackData attackData, CNWSItem weapon)
    {
      int attackBonus = attacker.m_pStats.GetAttackModifierVersus(target);

      if (attackData.m_bRangedAttack.ToBool() && attacker.m_pStats.HasFeat(CustomSkill.FighterCombatStyleArchery).ToBool())
      {
        attackBonus += 2;
        LogUtils.LogMessage("Style de combat archerie : +2 BA", LogUtils.LogType.Combat);
      }

      var initiaLocation = attacker.m_pStats.m_pBaseCreature.m_ScriptVars.GetLocation(EffectSystem.chargerVariableExo);

      if (initiaLocation.m_oArea != NWScript.OBJECT_INVALID && Vector3.DistanceSquared(initiaLocation.m_vPosition.ToManagedVector(), attacker.m_vPosition.ToManagedVector()) > 9)
      {
        attackBonus += 5;

        foreach (var eff in attacker.m_appliedEffects)
          if (eff.m_sCustomTag.CompareNoCase(EffectSystem.chargeurEffectExoTag).ToBool())
            attacker.RemoveEffect(eff);

        BroadcastNativeServerMessage("Charge (+5 BA)".ColorString(ColorConstants.Orange), attacker);
        LogUtils.LogMessage("Chargeur : +5 BA", LogUtils.LogType.Combat);
      }

      attacker.m_pStats.m_pBaseCreature.m_ScriptVars.DestroyLocation(EffectSystem.chargerVariableExo);

      if (weapon is not null)
      {
        if (IsCogneurLourd(attacker, weapon))
        {
          attackBonus -= 5;
          LogUtils.LogMessage("Cogneur Lourd : -5 BA", LogUtils.LogType.Combat);
        }
        else if (IsTireurDelite(attacker, attackData, weapon))
        {
          attackBonus -= 5;
          LogUtils.LogMessage("Tireur d'élite : -5 BA", LogUtils.LogType.Combat);
        }
      }

      if(attacker.m_appliedEffects.Any(e => e.m_sCustomTag.CompareNoCase(EffectSystem.RecklessAttackEffectExoTag).ToBool()))
      {
        int boonBonus = NwRandom.Roll(Utils.random, 4);
        attackBonus += boonBonus;
        LogUtils.LogMessage($"Magie Sauvage : +{boonBonus} BA", LogUtils.LogType.Combat);
      }

      int frappeFrenetiqueMalus = attacker.m_pStats.m_pBaseCreature.m_ScriptVars.GetInt(CreatureUtils.FrappeFrenetiqueMalusVariableExo);
      if(frappeFrenetiqueMalus > 0)
      {
        attackBonus -= frappeFrenetiqueMalus;
        LogUtils.LogMessage($"Frappe Frénétique : - {frappeFrenetiqueMalus} BA", LogUtils.LogType.Combat);
      }
      
      return attackBonus;
    }
  }
}
