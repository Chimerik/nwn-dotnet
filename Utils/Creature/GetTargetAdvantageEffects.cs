using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetTargetAdvantageEffects(CNWSCreature target, CNWSCreature attacker, bool rangedAttack)
    {
      foreach (var eff in target.m_appliedEffects)
      {
        EffectTrueType effectType = (EffectTrueType)eff.m_nType;
        int statParameter = eff.GetInteger(0);

        switch (effectType)
        {
          case EffectTrueType.SetState:

            switch(statParameter)
            {
              case EffectState.Etourdi: LogUtils.LogMessage("Avantage - Cible étourdie", LogUtils.LogType.Combat); return true;
              case EffectState.Paralyse: LogUtils.LogMessage("Avantage - Cible paralysée", LogUtils.LogType.Combat); return true;
            }

            break;

          case EffectTrueType.Blindness: 
          case EffectTrueType.Darkness: if(GetBlindedAdvantage(attacker, target))  return true; break;
          case EffectTrueType.Petrify: LogUtils.LogMessage("Avantage - Cible pétrifiée", LogUtils.LogType.Combat); return true;
        }

        string tag = eff.m_sCustomTag.ToString();
        uint effCreator = eff.m_oidCreator;

        switch (tag)
        {
          case EffectSystem.KnockdownEffectTag: if (GetKnockdownMeleeAdvantage(tag, rangedAttack)) return true; break;
          case EffectSystem.MarqueDuChasseurTag: if (GetRangerPrecisAdvantage(tag, effCreator, attacker)) return true; break;
          case EffectSystem.FrappeEtourdissanteEffectTag: LogUtils.LogMessage("Avantage - Frappe Etourdissante", LogUtils.LogType.Combat); return true;
          case EffectSystem.EntraveEffectTag: LogUtils.LogMessage("Avantage -  Cible Entravée", LogUtils.LogType.Combat); return true;
          case EffectSystem.faerieFireEffectTag: LogUtils.LogMessage("Avantage -  Cible affectée par Lueurs Féeriques", LogUtils.LogType.Combat); return true;
          case EffectSystem.RecklessAttackEffectTag: LogUtils.LogMessage("Avantage - Contre Frappe Téméraire", LogUtils.LogType.Combat); return true;
          case EffectSystem.WolfTotemEffectTag: LogUtils.LogMessage("Avantage - Totem de l'esprit du Loup", LogUtils.LogType.Combat); return true;
          case EffectSystem.MauvaisAugureEffectTag: LogUtils.LogMessage("Avantage - Mauvais Augure", LogUtils.LogType.Combat); return true;
          case EffectSystem.CourrouxDeLaNatureEffectTag: LogUtils.LogMessage("Avantage - Cible affecté par Courroux de la Nature", LogUtils.LogType.Combat); return true;
          case EffectSystem.MoulinetEffectTag: LogUtils.LogMessage("Avantage - Moulinet", LogUtils.LogType.Combat); return true;
          case EffectSystem.ChargeDebuffEffectTag: LogUtils.LogMessage("Avantage - Charge", LogUtils.LogType.Combat); return true;
          case EffectSystem.ArcaneTricksterPolyvalentEffectTag: if(GetPolyvalentTricksterAdvantage(tag, effCreator, attacker)) return true; break;
          case EffectSystem.VoeuDHostiliteEffectTag: if(GetVoeudHostiliteAdvantage(tag, effCreator, attacker)) return true; break;
          case EffectSystem.RepliqueInvoqueeEffectTag: if(GetRepliqueDupliciteAdvantage(tag, effCreator, attacker)) return true; break;
        }
      }

      return false;
    }
  }
}
