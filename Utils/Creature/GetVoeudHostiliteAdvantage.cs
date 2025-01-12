using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetVoeudHostiliteAdvantage(string tag, uint effCreator, CNWSCreature attacker)
    {
      if(effCreator == attacker.m_idSelf)
      {
        LogUtils.LogMessage("Avantage - Voeu d'Hostilité", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
