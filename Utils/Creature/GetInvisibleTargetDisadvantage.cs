using NWN.Native.API;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static bool GetInvisibleTargetDisadvantage(CNWSCreature attacker, CNWSCreature target)
    {
      if (attacker.GetVisibleListElement(target.m_idSelf) is null
        || attacker.GetVisibleListElement(target.m_idSelf).m_bSeen < 1
        || attacker.GetVisibleListElement(target.m_idSelf).m_bInvisible > 0)
      {
        LogUtils.LogMessage("Désavantage - Cible non visible", LogUtils.LogType.Combat);
        return true;
      }
      else 
        return false;
    }
  }
}
