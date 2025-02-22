using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static bool IsIllusionDouble(CNWSCreature attacker, CNWSCreature target, CNWSCombatRound combatRound, string attackerName, string targetName)
    {
      if (!target.m_pStats.HasFeat(CustomSkill.IllusionDouble).ToBool())
        return false;

      var reaction = target.m_appliedEffects.FirstOrDefault(e => e.m_sCustomTag.ToString() == EffectSystem.ReactionEffectTag);

      if (reaction is null)
        return false;

      bool doubleTrigger = false;

      foreach (var eff in target.m_appliedEffects)
      {
        if (eff.m_sCustomTag.ToString() == EffectSystem.IllusionDoubleEffectTag)
        {
          doubleTrigger = true;
          DelayEffectRemoval(target, eff);
        }
      }

      if (doubleTrigger)
      {
        LogUtils.LogMessage($"Echec automatique - Double Illusoire de {targetName}", LogUtils.LogType.Combat);
        BroadcastNativeServerMessage($"{attackerName.ColorString(ColorConstants.Cyan)} touche le Double Illusoire de {targetName.ColorString(ColorConstants.Cyan)}".ColorString(StringUtils.gold), target);
        target.RemoveEffect(reaction);
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
