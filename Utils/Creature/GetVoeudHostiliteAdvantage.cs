using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetVoeudHostiliteAdvantage(CGameEffect eff, CNWSCreature attacker)
    {
      if(eff.m_sCustomTag.CompareNoCase(EffectSystem.VoeuDHostiliteEffectExoTag).ToBool()
        && eff.m_oidCreator == attacker.m_idSelf)
      {
        LogUtils.LogMessage("Avantage - Voeu d'Hostilité", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
