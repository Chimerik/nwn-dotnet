using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetRangerPrecisAdvantage(string tag, uint effCreator, CNWSCreature attacker)
    {
      if(attacker.m_pStats.HasFeat(CustomSkill.RangerPrecis).ToBool() 
        && tag == EffectSystem.MarqueDuChasseurTag
        && effCreator == attacker.m_idSelf)
      {
        LogUtils.LogMessage("Avantage - Chasseur Précis", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
