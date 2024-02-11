using Anvil.API;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static bool GetWolfTotemAttackAdvantage(Native.API.CGameEffect eff)
    {
      if(eff.m_sCustomTag.CompareNoCase(EffectSystem.WolfTotemEffectExoTag).ToBool())
      {
        LogUtils.LogMessage("Avantage - Totem de l'esprit du Loup", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
