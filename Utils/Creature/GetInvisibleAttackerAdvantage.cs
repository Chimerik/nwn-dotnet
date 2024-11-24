using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetInvisibleAttackerAdvantage(CNWSCreature attacker, CNWSCreature target)
    {
      //if(target.m_pStats.HasFeat(CustomSkill.Vigilant).ToBool())
        //return false;

      if (target.GetVisibleListElement(attacker.m_idSelf) is null
        || target.GetVisibleListElement(attacker.m_idSelf).m_bSeen < 1
        || target.GetVisibleListElement(attacker.m_idSelf).m_bInvisible > 0)
      {
        LogUtils.LogMessage("Avantage - Attaquant non visible", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
