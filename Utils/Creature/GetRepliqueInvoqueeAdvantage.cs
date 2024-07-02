using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetRepliqueInvoqueeAdvantage(Native.API.CGameEffect eff)
    {
      if(eff.m_sCustomTag.CompareNoCase(EffectSystem.RepliqueInvoqueeEffectExoTag).ToBool())
      {
        LogUtils.LogMessage("Avantage - CIble affectée par Réplique Invoquée", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
