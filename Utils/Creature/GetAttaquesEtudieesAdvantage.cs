using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void GetAttaquesEtudieesAdvantage(CGameEffect eff, CNWSCreature attacker)
    {
      LogUtils.LogMessage("Avantage - Attaque Etudiees", LogUtils.LogType.Combat);
      EffectUtils.DelayEffectRemoval(attacker, eff);
    }
  }
}
