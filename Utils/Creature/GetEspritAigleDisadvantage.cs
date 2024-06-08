using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetEspritAigleDisadvantage(CGameEffect eff, CNWSCreature target, CNWSCombatAttackData attackData)
    {
      if (attackData is null || attackData.m_nAttackType != 65002 || !target.m_pStats.HasFeat(CustomSkill.TotemEspritAigle).ToBool()
        || !eff.m_sCustomTag.CompareNoCase(EffectSystem.barbarianRageEffectExoTag).ToBool()) // 65002 = attaque d'opportunité
        return false;

      LogUtils.LogMessage("Désavantage - Esprit de l'Aigle vs Attaque d'opportunité", LogUtils.LogType.Combat);
      return true;
    }
  }
}
