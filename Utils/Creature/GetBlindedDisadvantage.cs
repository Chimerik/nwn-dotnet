using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetBlindedDisadvantage(CNWSCreature creature, CGameEffect eff)
    {
      if (!creature.m_pStats.HasFeat(CustomSkill.RangerSensSauvages).ToBool() && (EffectTrueType)eff.m_nType == EffectTrueType.Blindness || (EffectTrueType)eff.m_nType == EffectTrueType.Darkness)
      {
        LogUtils.LogMessage("Désavantage - Attaquant aveuglé ou subissant Ténèbres", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
