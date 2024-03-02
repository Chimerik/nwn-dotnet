using Anvil.API;
using NWN.Native.API;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static bool GetHighGroundAdvantage(CNWSCreature attacker, CNWSCreature target)
    {
      float distance = attacker.m_vPosition.z - target.m_vPosition.z;

      if (distance > 3)
      {
        LogUtils.LogMessage($"Avantage - Attaque à distance en surélévation", LogUtils.LogType.Combat);
        return true;
      }

      return false;
    }
  }
}
