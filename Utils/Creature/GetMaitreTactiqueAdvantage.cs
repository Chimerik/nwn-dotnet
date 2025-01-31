using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void GetMaitreTactiqueAdvantage(CGameEffect eff, CNWSCreature attacker)
    {
      attacker.RemoveEffect(eff);
      LogUtils.LogMessage("Avantage - Maître Tactique", LogUtils.LogType.Combat);
    }
  }
}
