using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static bool IsIllusionDouble(CNWSCreature attacker, CNWSCreature target, CNWSCombatRound combatRound, string attackerName)
    {
      if (!target.m_pStats.HasFeat(CustomSkill.IllusionDouble).ToBool()
        || target.m_ScriptVars.GetInt(CreatureUtils.ReactionVariableExo) < 1
        || !target.m_appliedEffects.Any(e => e.m_sCustomTag.CompareNoCase(EffectSystem.IllusionDoubleEffectExoTag).ToBool()))
        return false;

      bool doubleTrigger = false;

      foreach (var eff in target.m_appliedEffects)
      {
        if (eff.m_sCustomTag.CompareNoCase(EffectSystem.IllusionDoubleEffectExoTag).ToBool())
        {
          doubleTrigger = true;
          target.RemoveEffect(eff);
        }
      }

      if(doubleTrigger)
        BroadcastNativeServerMessage("Double illusoire".ColorString(StringUtils.gold), target);

      return doubleTrigger;
    }
  }
}
