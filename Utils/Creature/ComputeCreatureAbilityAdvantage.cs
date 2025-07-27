using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool ComputeCreatureAbilityAdvantage(NwCreature creature, Ability ability, int skill = -1)
    {
      switch (ability)
      {
        case Ability.Strength:

          if(skill == CustomSkill.AthleticsProficiency && creature.KnowsFeat((Feat)CustomSkill.FighterChampionAthleteAccompli))
            return true;

          break;

        case Ability.Dexterity:

          if (skill == CustomSkill.StealthProficiency)
          {
            if (creature.KnowsFeat((Feat)CustomSkill.ThiefDiscretionSupreme))
            {
              LogUtils.LogMessage("Avantage - Voleur : Discrétion Suprème", LogUtils.LogType.Combat);
              return true;
            }
            else if (creature.KnowsFeat((Feat)CustomSkill.Traqueur1) && creature.Area.IsNatural)
            {
              LogUtils.LogMessage("Avantage - Trucs de Rôdeur : Traqueur I", LogUtils.LogType.Combat);
              return true;
            }
          }

          break;
        }

      foreach (var eff in creature.ActiveEffects)
      {
        switch(eff.Tag)
        {
          case EffectSystem.ChanceuxAvantageEffectTag:
            LogUtils.LogMessage("Avantage - Chanceux", LogUtils.LogType.Combat);
            creature.RemoveEffect(eff);
            return true;

          case EffectSystem.InspirationHeroiqueEffectTag: 
            LogUtils.LogMessage("Avantage - Inspiration Heroïque", LogUtils.LogType.Combat);
            creature.RemoveEffect(eff);
            return true;
        }

        switch (ability)
        {
          case Ability.Strength:

            switch (eff.Tag)
            {
              case EffectSystem.RageDuSanglierEffectTag:
              case EffectSystem.BarbarianRageEffectTag:
                LogUtils.LogMessage("Avantage - Barbare : Rage", LogUtils.LogType.Combat);
                return true;
              case EffectSystem.EnlargeEffectTag: LogUtils.LogMessage("Avantage - Agrandissement", LogUtils.LogType.Combat); return true;
              
              case EffectSystem.AmeliorationCaracteristiqueEffectTag:
                
                if (eff.Spell == NwSpell.FromSpellId(CustomSpell.AmeliorationForce))
                { 
                  LogUtils.LogMessage("Avantage - Force du Taureau", LogUtils.LogType.Combat); 
                  return true; 
                }

                break;
            }

            break;

          case Ability.Dexterity:

            switch(eff.Tag)
            {
              case EffectSystem.AmeliorationCaracteristiqueEffectTag:

                if (eff.Spell == NwSpell.FromSpellId(CustomSpell.AmeliorationDexterite))
                {
                  LogUtils.LogMessage("Avantage - Grâce Féline", LogUtils.LogType.Combat);
                  return true;
                }

                break;
            }

            break;

          case Ability.Constitution:

            switch(eff.Tag)
            {
              case EffectSystem.AmeliorationCaracteristiqueEffectTag:

                if (eff.Spell == NwSpell.FromSpellId(CustomSpell.AmeliorationConstitution))
                {
                  LogUtils.LogMessage("Avantage - Endurance de l'Ours", LogUtils.LogType.Combat);
                  return true;
                }

                break;
            }

            break;

          case Ability.Intelligence:

            switch (eff.Tag)
            {
              case EffectSystem.AmeliorationCaracteristiqueEffectTag:

                if (eff.Spell == NwSpell.FromSpellId(CustomSpell.AmeliorationIntelligence))
                {
                  LogUtils.LogMessage("Avantage - Ruse du Renard", LogUtils.LogType.Combat);
                  return true;
                }

                break;
            }

            break;

          case Ability.Wisdom:

            switch (eff.Tag)
            {
              case EffectSystem.AmeliorationCaracteristiqueEffectTag:

                if (eff.Spell == NwSpell.FromSpellId(CustomSpell.AmeliorationSagesse))
                {
                  LogUtils.LogMessage("Avantage - Sagesse du Hibou", LogUtils.LogType.Combat);
                  return true;
                }

                break;
            }

            break;

          case Ability.Charisma:

            switch (eff.Tag)
            {
              case EffectSystem.AmeliorationCaracteristiqueEffectTag:

                if (eff.Spell == NwSpell.FromSpellId(CustomSpell.AmeliorationCharisme))
                {
                  LogUtils.LogMessage("Avantage - Splendeur de l'Aigle", LogUtils.LogType.Combat);
                  return true;
                }

                break;
            }

            break;
        }

        switch(skill)
        {
          case CustomSkill.StealthProficiency:

            switch(eff.Tag)
            {
              case EffectSystem.BenedictionDuFilouEffectTag:
                LogUtils.LogMessage("Avantage - Bénédiction du Filou", LogUtils.LogType.Combat);
                return true;
            }

            break;

          case CustomSkill.PerceptionProficiency:

            switch (eff.Tag)
            {
              case EffectSystem.MarqueDuChasseurTag:

                if (eff.Creator == creature)
                {
                  LogUtils.LogMessage("Avantage - Perception vs cible sous Marque du Chasseur", LogUtils.LogType.Combat);
                  return true;
                }

                break;
            }

            break;
        }
      }

      return false;
    }
  }
}
