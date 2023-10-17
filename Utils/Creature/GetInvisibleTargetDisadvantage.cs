using NWN.Native.API;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static int GetInvisibleTargetDisadvantage(CNWSCreature attacker, CNWSCreature target)
    {
      return (attacker.GetVisibleListElement(target.m_idSelf) is null
        || attacker.GetVisibleListElement(target.m_idSelf).m_bSeen < 1
        || attacker.GetVisibleListElement(target.m_idSelf).m_bInvisible > 0)
        ? -1 : 0;
    }
  }
}
