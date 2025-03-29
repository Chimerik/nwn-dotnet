using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetAssassinateAdvantage(CNWSCreature attacker, CNWSCreature target)
    {
      if(attacker.m_nInitiativeRoll > target.m_nInitiativeRoll)
      { 
        LogUtils.LogMessage("Avantage - Assassinat", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;        
    }
  }
}
