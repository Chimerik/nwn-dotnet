using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetGuerrierHeroiqueAdvantage(CNWSCreature attacker)
    {
      if(attacker.m_pStats.HasFeat(CustomSkill.ChampionGuerrierHeroique).ToBool() && attacker.m_pcCombatRound.m_nCurrentAttack == 0)
      {
        LogUtils.LogMessage("Avantage - Guerrier Héroique", LogUtils.LogType.Combat);
        return true;
      }

      return false;
    }
  }
}
