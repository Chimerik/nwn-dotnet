using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetKnockdownMeleeAdvantage(CGameEffect eff, bool rangedAttack)
    {
      if (rangedAttack || !eff.m_sCustomTag.CompareNoCase(EffectSystem.KnockdownEffectTagExo).ToBool())
        return false;

      LogUtils.LogMessage("Avantage - Attaque de mêlée sur une cible Déstabilisée", LogUtils.LogType.Combat);
      return true;
    }
  }
}
