using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetKnockdownRangedDisadvantage(CGameEffect eff, bool rangedAttack)
    {
      if (!rangedAttack || !eff.m_sCustomTag.CompareNoCase(EffectSystem.KnockdownEffectTagExo).ToBool())
        return false;

      LogUtils.LogMessage("Désavantage - Attaque à distance sur une cible Déstabilisée", LogUtils.LogType.Combat);
      return true;    
    }
  }
}
