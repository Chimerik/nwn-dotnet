using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetAffaiblissementDisadvantage(CNWSCreature creature, CGameEffect eff)
    {
      if(eff.m_sCustomTag.CompareNoCase(EffectSystem.AffaiblissementEffectExoTag).ToBool())
      {
        LogUtils.LogMessage("Désavantage - Affecté par Affaiblissement", LogUtils.LogType.Combat);
        creature.RemoveEffect(eff);
        return true;
      }
      else
        return false;
    }
  }
}
