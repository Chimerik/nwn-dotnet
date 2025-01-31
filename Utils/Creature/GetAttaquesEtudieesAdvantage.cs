using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetAttaquesEtudieesAdvantage(CGameEffect eff, CNWSCreature attacker, CNWSCreature target)
    {
      if (eff.m_oidCreator == target.m_idSelf)
      {
        LogUtils.LogMessage("Avantage - Attaque Etudiees", LogUtils.LogType.Combat);
        attacker.RemoveEffect(eff);
        return true;
      }

      return false;
    }
  }
}
