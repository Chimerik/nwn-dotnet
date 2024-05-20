using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetHunterEchapperAlaHordeDisadvantage(CNWSCreature target, CNWSCombatAttackData attackData)
    {
      if (attackData.m_nAttackType != 65002 || !target.m_pStats.HasFeat(CustomSkill.ChasseurEchapperAlaHorde).ToBool())
        return false;

      LogUtils.LogMessage("Désavantage - Echapper à la horde", LogUtils.LogType.Combat);

      return true;
    }
  }
}
