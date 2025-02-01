using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static bool IsConspirateurRedirection(CNWSCreature attacker, CNWSCreature target, CNWSCombatRound combatRound, string attackerName)
    {
      if (!target.m_pStats.HasFeat(CustomSkill.ConspirateurRedirection).ToBool())
        return false;

      var reaction = target.m_appliedEffects.FirstOrDefault(e => e.m_sCustomTag.ToString() == EffectSystem.ReactionEffectTag);

      if (reaction is null)
        return false;

      var newTarget = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(target.GetNearestEnemy(2, attacker.m_idSelf, 1, 1));

      if (newTarget is null || newTarget.m_idSelf == 0x7F000000)
        return false;

      string newTargetName = $"{newTarget.GetFirstName().GetSimple(0)} {newTarget.GetLastName().GetSimple(0)}".ColorString(ColorConstants.Cyan);
      string targetName = $"{target.GetFirstName().GetSimple(0)} {target.GetLastName().GetSimple(0)}".ColorString(ColorConstants.Cyan);
      BroadcastNativeServerMessage($"{targetName.ColorString(ColorConstants.Cyan)} redirige l'attaque de {attackerName.ColorString(ColorConstants.Cyan)} sur {newTargetName.ColorString(ColorConstants.Cyan)}", target);

      combatRound.AddWhirlwindAttack(newTarget.m_idSelf, 1);
      target.RemoveEffect(reaction);
      LogUtils.LogMessage($"Attaque redirigée vers {newTargetName.StripColors()}", LogUtils.LogType.Combat);

      return true;
    }
  }
}
