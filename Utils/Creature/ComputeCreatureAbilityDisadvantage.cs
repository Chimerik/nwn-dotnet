﻿using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool ComputeCreatureAbilityDisadvantage(NwCreature creature, Ability ability, int skill)
    {
      foreach (var eff in creature.ActiveEffects)
      {
        switch (eff.Tag)
        {
          case EffectSystem.RayonAffaiblissantDesavantageEffectTag:

            if (ability == Ability.Strength)
            {
              LogUtils.LogMessage("Désavantage - Rayon Affaiblissant", LogUtils.LogType.Combat);
              return true;
            }

            break;

          case EffectSystem.MaleficeTag:

            if(eff.Spell is not null)
            {
              switch(eff.Spell.Id)
              {
                case CustomSpell.MaledictionForce: if (ability == Ability.Strength) return true; break;
                case CustomSpell.MaledictionDexterite: if (ability == Ability.Dexterity) return true; break;
                case CustomSpell.MaledictionConstitution: if (ability == Ability.Constitution) return true; break;
                case CustomSpell.MaledictionIntelligence: if (ability == Ability.Intelligence) return true; break;
                case CustomSpell.MaledictionSagesse: if (ability == Ability.Wisdom) return true; break;
                case CustomSpell.MaledictionCharisme: if (ability == Ability.Charisma) return true; break;
              }
            }

            break;

          case EffectSystem.FrightenedEffectTag:
            LogUtils.LogMessage("Désavantage - Effroi", LogUtils.LogType.Combat);
            return true;

          case EffectSystem.PoisonEffectTag:
            LogUtils.LogMessage("Désavantage - Poison", LogUtils.LogType.Combat);
            return true;
        }

        switch (ability)
        {
          case Ability.Strength:

            switch(eff.Tag)
            {
              case EffectSystem.ShieldArmorDisadvantageEffectTag: LogUtils.LogMessage("Désavantage - Bouclier ou Armure non maîtrisé", LogUtils.LogType.Combat); return true;
              case EffectSystem.MaledictionForceEffectTag: LogUtils.LogMessage("Désavantage - Malédiction Force", LogUtils.LogType.Combat); return true;
              case EffectSystem.KnockdownEffectTag: LogUtils.LogMessage("Désavantage - Déstabilisé", LogUtils.LogType.Combat); return true;
              case EffectSystem.MoulinetEffectTag: LogUtils.LogMessage("Désavantage - Moulinet", LogUtils.LogType.Combat); return true;
              case EffectSystem.ChargeDebuffEffectTag: LogUtils.LogMessage("Désavantage - Charge", LogUtils.LogType.Combat); return true;
              case EffectSystem.RapetissementEffectTag: LogUtils.LogMessage("Désavantage - Rapetissement", LogUtils.LogType.Combat); return true;
            }

            break;

          case Ability.Dexterity:

            switch(eff.Tag)
            {
              case EffectSystem.ShieldArmorDisadvantageEffectTag: LogUtils.LogMessage("Désavantage - Bouclier ou Armure non maîtrisé", LogUtils.LogType.Combat); return true;
              case EffectSystem.CourrouxDeLaNatureEffectTag: LogUtils.LogMessage("Désavantage - Courroux de la Nature", LogUtils.LogType.Combat); return true;
              case EffectSystem.MaledictionDexteriteEffectTag: LogUtils.LogMessage("Désavantage - Malédiction Dextérité", LogUtils.LogType.Combat); return true;
              case EffectSystem.KnockdownEffectTag: LogUtils.LogMessage("Désavantage - Déstabilisé", LogUtils.LogType.Combat); return true;
              case EffectSystem.MoulinetEffectTag: LogUtils.LogMessage("Désavantage - Moulinet", LogUtils.LogType.Combat); return true;
              case EffectSystem.MutilationEffectTag: LogUtils.LogMessage("Désavantage - Mutilation", LogUtils.LogType.Combat); return true;
              case EffectSystem.ChargeDebuffEffectTag: LogUtils.LogMessage("Désavantage - Charge", LogUtils.LogType.Combat); return true;
            }

            break;

          case Ability.Constitution:

            switch(eff.Tag)
            {
              case EffectSystem.MorsureInfectieuseEffectTag: LogUtils.LogMessage("Désavantage - Saignement ou Morsure Infectieuse", LogUtils.LogType.Combat); return true;
              case EffectSystem.MaledictionConstitutionEffectTag: LogUtils.LogMessage("Désavantage - Malédiction Constitution", LogUtils.LogType.Combat); return true;
              case EffectSystem.ArretCardiaqueEffectTag: LogUtils.LogMessage("Désavantage - Arrêt Cardiaque", LogUtils.LogType.Combat); return true;
            }

            break;

          case Ability.Intelligence:

            if (EffectSystem.MaledictionIntelligenceEffectTag == eff.Tag)
            {
              LogUtils.LogMessage("Désavantage - Malédiction Intelligence", LogUtils.LogType.Combat);
              return true;
            }

            break;

          case Ability.Wisdom:

            if (EffectSystem.MaledictionSagesseEffectTag == eff.Tag)
            {
              LogUtils.LogMessage("Désavantage - Malédiction Sagesse", LogUtils.LogType.Combat);
              return true;
            }
            else if (EffectSystem.HebetementEffectTag == eff.Tag)
            {
              LogUtils.LogMessage("Désavantage - Hébétement", LogUtils.LogType.Combat);
              return true;
            }

            break;
        }
      }

      return false;
    }
  }
}
