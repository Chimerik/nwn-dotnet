using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetWolfPackAttackerAdvantage(CNWSCreature attacker, CNWSCreature target)
    {
      var master = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(attacker.m_oidMaster);

      if (master is null || master.m_idSelf == 0x7F000000 || !master.m_pStats.HasFeat(CustomSkill.BelluaireLoupEffetDeMeute).ToBool()
        || master.m_oidAttackTarget != target.m_idSelf)
        return false;

      LogUtils.LogMessage("Avantage - Effet de Meute", LogUtils.LogType.Combat);
      return true;
    }
  }
}
