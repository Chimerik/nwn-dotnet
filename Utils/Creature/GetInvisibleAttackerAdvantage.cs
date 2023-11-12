using Anvil.API;
using NWN.Native.API;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static int GetInvisibleAttackerAdvantage(CNWSCreature attacker, CNWSCreature target)
    {
      if(target.m_pStats.HasFeat(CustomSkill.Vigilant) > 0)
        return 0;

      return (target.GetVisibleListElement(attacker.m_idSelf) is null
        || target.GetVisibleListElement(attacker.m_idSelf).m_bSeen < 1
        || target.GetVisibleListElement(attacker.m_idSelf).m_bInvisible > 0)
        ? 1 : 0;
    }
  }
}
