using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetMaledictionAttaqueDisadvantage(CGameEffect eff, CNWSCreature target)
    {
      if (eff.m_sCustomTag.CompareNoCase(EffectSystem.MaledictionAttaqueEffectExoTag).ToBool() && eff.m_oidCreator == target.m_idSelf)
        return false;

      LogUtils.LogMessage("Désavantage - Malédiction sur l'attaque", LogUtils.LogType.Combat);
      return true;
    }
  }
}
