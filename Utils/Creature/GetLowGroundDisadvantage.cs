using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetLowGroundDisadvantage(CNWSCreature attacker, CNWSCreature target)
    {
      float distance = attacker.m_vPosition.z - target.m_vPosition.z;

      if (attacker.m_pStats.HasFeat(CustomSkill.TireurDelite).ToBool())
        return false;

      if (distance < -3)
      {
        LogUtils.LogMessage($"Désavantage - Attaque à distance en sousélévation", LogUtils.LogType.Combat);
        return true;
      }

      return false;
    }
  }
}
