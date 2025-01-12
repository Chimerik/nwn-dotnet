using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetAttackerDisadvantageEffects(CNWSCreature attacker, CNWSCreature targetId, Anvil.API.Ability attackStat, CNWSItem attackWeapon = null)
    {
      foreach (var eff in attacker.m_appliedEffects)
      {
        EffectTrueType effectType = (EffectTrueType)eff.m_nType;

        switch(effectType)
        {
          case EffectTrueType.Blindness:
          case EffectTrueType.Darkness: if (GetBlindedDisadvantage(attacker, targetId)) return true; break;
          case EffectTrueType.Poison: LogUtils.LogMessage("Désavantage - Empoisonné", LogUtils.LogType.Combat); return true;
        }

        string tag = eff.m_sCustomTag.ToString();
        uint effCreator = eff.m_oidCreator;

        switch (tag)
        {
          case EffectSystem.PoisonEffectTag: LogUtils.LogMessage("Désavantage - Empoisonné", LogUtils.LogType.Combat); return true;
          case EffectSystem.lightSensitivityEffectTag: LogUtils.LogMessage("Désavantage - Drow en pleine lumière", LogUtils.LogType.Combat); return true;
          case EffectSystem.PourfendeurDisadvantageEffectTag: LogUtils.LogMessage("Désavantage - Affecté par Pourfendeur", LogUtils.LogType.Combat); return true;
          case EffectSystem.EntraveEffectTag: LogUtils.LogMessage("Désavantage - Entravé", LogUtils.LogType.Combat); return true;
          case EffectSystem.FrightenedEffectTag: LogUtils.LogMessage("Désavantage - Effroi", LogUtils.LogType.Combat); return true;
          case EffectSystem.CourrouxDeLaNatureEffectTag: LogUtils.LogMessage("Désavantage - Cible affecté par Courroux de la Nature", LogUtils.LogType.Combat); return true;
          case EffectSystem.ThreatenedEffectTag: if (GetThreatenedDisadvantage(attacker, attackWeapon)) return true; break;
          case EffectSystem.MoquerieVicieuseEffectTag: if (GetMoquerieVicieuseDisadvantage(attacker, eff)) return true; break;
          case EffectSystem.ShieldArmorDisadvantageEffectTag: if (GetArmorShieldDisadvantage(attackStat)) return true; break;
          case EffectSystem.boneChillEffectTag: if (GetBoneChillDisadvantage(attacker)) return true; break;
          case EffectSystem.AffaiblissementEffectTag: if (GetAffaiblissementDisadvantage(attacker, eff)) return true; break;
          case EffectSystem.DuelForceEffectTag: if (GetDuelForceDisadvantage(effCreator, targetId)) return true; break;
          case EffectSystem.ProvocationEffectTag: if (GetProvocationDisadvantage(effCreator, targetId)) return true; break;
          case EffectSystem.EspritEveilleEffectTag: if (GetEspritEveilleDisadvantage(effCreator, targetId.m_idSelf)) return true; break;
        }
      }

      return false;
    }
  }
}
