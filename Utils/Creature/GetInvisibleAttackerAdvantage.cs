using NWN.Native.API;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static int GetInvisibleAttackerAdvantage(CNWSCreature attacker, CNWSCreature target)
    {
      return (target.GetVisibleListElement(attacker.m_idSelf) is null
        || target.GetVisibleListElement(attacker.m_idSelf).m_bSeen < 1
        || target.GetVisibleListElement(attacker.m_idSelf).m_bInvisible > 0)
        ? 1 : 0;
    }
  }
}
