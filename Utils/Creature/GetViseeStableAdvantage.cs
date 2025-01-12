using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void GetViseeStableAdvantage(CGameEffect eff, CNWSCreature attacker)
    {
      EffectUtils.DelayEffectRemoval(attacker, eff);
      LogUtils.LogMessage("Avantage - Visee Stable", LogUtils.LogType.Combat);
    }
  }
}
