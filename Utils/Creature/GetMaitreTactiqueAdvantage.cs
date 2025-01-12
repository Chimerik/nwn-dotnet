using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void GetMaitreTactiqueAdvantage(CGameEffect eff, CNWSCreature attacker)
    {
      EffectUtils.DelayEffectRemoval(attacker, eff);
      LogUtils.LogMessage("Avantage - Maître Tactique", LogUtils.LogType.Combat);
    }
  }
}
