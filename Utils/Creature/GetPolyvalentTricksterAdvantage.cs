using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetPolyvalentTricksterAdvantage(Native.API.CGameEffect eff, CNWSCreature attacker)
    {
      if(eff.m_sCustomTag.CompareNoCase(EffectSystem.arcaneTricksterPolyvalentEffectExoTag).ToBool()
        && eff.m_oidCreator == attacker.m_idSelf)
      {
        LogUtils.LogMessage("Avantage - Escroc Polyvalent", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
