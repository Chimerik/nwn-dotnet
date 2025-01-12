using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetTargetDisadvantageEffects(CNWSCreature attacker, CNWSCreature target, bool rangedAttack, CNWSCombatAttackData data = null)
    {
      foreach (var eff in target.m_appliedEffects)
      {
        string tag = eff.m_sCustomTag.ToString();
        uint effCreator = eff.m_oidCreator;

        switch(tag)
        {
          case EffectSystem.DodgeEffectTag: LogUtils.LogMessage("Désavantage - Cible en mode esquive", LogUtils.LogType.Combat); return true;
          case EffectSystem.FlouEffectTag: LogUtils.LogMessage("Désavantage - Cible Floue", LogUtils.LogType.Combat); return true;
          case EffectSystem.ProtectionStyleEffectTag: LogUtils.LogMessage("Désavantage - Cible sous Protection (Guerrier)", LogUtils.LogType.Combat); return true;
          case EffectSystem.JeuDeJambeEffectTag: LogUtils.LogMessage("Désavantage - Cible en mode jeu de jambe", LogUtils.LogType.Combat); return true;
          case EffectSystem.MonkPatienceEffectTag: LogUtils.LogMessage("Désavantage - Cible en mode patience", LogUtils.LogType.Combat); return true;
          case EffectSystem.KnockdownEffectTag: if (GetKnockdownRangedDisadvantage(rangedAttack)) return true; break;
          case EffectSystem.IlluminationProtectriceEffectTag: if (GetIlluminationProtectriceDisadvantage(eff, target)) return true; break;
          case EffectSystem.ProtectionContreLeMalEtLeBienEffectTag: if (GetProtectionContreLeMalEtLeBienDisadvantage(attacker)) return true; break;
          case EffectSystem.MaledictionAttaqueEffectTag: if (GetMaledictionAttaqueDisadvantage(effCreator, target)) return true; break;
          case EffectSystem.EchapperALaHordeEffectTag: if (GetHunterEchapperAlaHordeDisadvantage(data)) return true; break;
          case EffectSystem.DefenseAdaptativeMalusEffectTag: if (GetDefenseAdaptativeDisadvantage(effCreator, target)) return true; break;
          case EffectSystem.VolEffectTag: if (GetVolMeleeDisadvantage(attacker, target, rangedAttack)) return true; break;
        }
      }

      return false;
    }
  }
}
