using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetHunterEchapperAlaHordeDisadvantage(CGameEffect eff, CNWSCombatAttackData attackData)
    {
      if (attackData is null || attackData.m_nAttackType != 65002 || !eff.m_sCustomTag.CompareNoCase(EffectSystem.EchapperALaHordeEffectExoTag).ToBool())
        return false;

      LogUtils.LogMessage("Désavantage - Echapper à la horde", LogUtils.LogType.Combat);

      return true;
    }
  }
}
