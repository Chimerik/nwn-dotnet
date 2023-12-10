using Anvil.API;
using NWN.Native.API;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static int GetHighGroundAdvantage(CNWSCreature attacker, CNWSCreature target)
    {
      float distance = attacker.m_vPosition.z - target.m_vPosition.z;

      if (distance > 3)
        return 1;

      if (attacker.m_pStats.HasFeat(CustomSkill.TireurDelite).ToBool())
        return 0;

      if (distance < -3)
        return -1;

      return 0;
    }
  }
}
