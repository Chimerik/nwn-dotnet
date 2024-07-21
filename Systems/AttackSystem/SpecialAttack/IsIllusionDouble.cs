using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static bool IsIllusionDouble(CNWSCreature attacker, CNWSCreature target, CNWSCombatRound combatRound, string attackerName, string targetName)
    {
      if (!target.m_pStats.HasFeat(CustomSkill.IllusionDouble).ToBool()
        || target.m_ScriptVars.GetInt(CreatureUtils.ReactionVariableExo) < 1)
        return false;

      bool doubleTrigger = false;

      foreach (var eff in target.m_appliedEffects)
      {
        if (eff.m_sCustomTag.CompareNoCase(EffectSystem.IllusionDoubleEffectExoTag).ToBool())
        {
          doubleTrigger = true;
          DelayEffectRemoval(target, eff);
        }
      }

      if (doubleTrigger)
      {
        LogUtils.LogMessage($"Echec automatique - Double Illusoire de {targetName}", LogUtils.LogType.Combat);
        BroadcastNativeServerMessage($"{attackerName.ColorString(ColorConstants.Cyan)} touche le Double Illusoire de {targetName.ColorString(ColorConstants.Cyan)}".ColorString(StringUtils.gold), target);
        target.m_ScriptVars.SetInt(CreatureUtils.ReactionVariableExo, target.m_ScriptVars.GetInt(CreatureUtils.ReactionVariableExo) - 1);
      }

      return doubleTrigger;
    }
    private static async void DelayEffectRemoval(CNWSCreature target, CGameEffect eff)
    {
      await NwTask.NextFrame();
      target.RemoveEffect(eff);
    }
  }
}
