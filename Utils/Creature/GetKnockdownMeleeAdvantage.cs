using NWN.Native.API;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static bool GetKnockdownMeleeAdvantage(CGameEffect eff, bool rangedAttack)
    {
      if (rangedAttack || (EffectTrueType)eff.m_nType != EffectTrueType.Knockdown)
        return false;

      LogUtils.LogMessage($"Avantage - Attaque de mêlée sur une cible à terre", LogUtils.LogType.Combat);
      return true;
    }
  }
}
