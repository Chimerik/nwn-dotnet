using System.Numerics;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetInvisibleTargetDisadvantage(CNWSCreature attacker, CNWSCreature target)
    {
      if (!attacker.m_pStats.HasFeat(CustomSkill.RangerSensSauvages).ToBool()
        && (!attacker.m_pStats.HasFeat(CustomSkill.FightingStyleCombatAveugle).ToBool() || Vector3.Distance(attacker.m_vPosition.ToManagedVector(), target.m_vPosition.ToManagedVector()) > 4)
        && (attacker.GetVisibleListElement(target.m_idSelf) is null
        || attacker.GetVisibleListElement(target.m_idSelf).m_bSeen < 1
        || attacker.GetVisibleListElement(target.m_idSelf).m_bInvisible > 0))
      {
        LogUtils.LogMessage("Désavantage - Cible non visible", LogUtils.LogType.Combat);
        return true;
      }
      else 
        return false;
    }
  }
}
