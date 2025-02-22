using System.Linq;
using Anvil.API;
using static NWN.Systems.SpellConfig;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool ComputeCreatureAbilityDisadvantage(NwCreature creature, Ability ability, SpellEntry spellEntry = null, SpellEffectType effectType = SpellEffectType.Invalid, NwGameObject oCaster = null)
    {
      if (oCaster is NwCreature caster)
      {
        if (caster.KnowsFeat((Feat)CustomSkill.ArcaneTricksterMagicalAmbush) && !creature.IsCreatureSeen(caster))
          return true;

        if (spellEntry is not null)
        {
          var spell = NwSpell.FromSpellId(spellEntry.RowIndex);
          byte paladinSpellLevel = spell.MasterSpell is null ? spell.GetSpellLevelForClass(ClassType.Paladin) : spell.MasterSpell.GetSpellLevelForClass(ClassType.Paladin);

          if (paladinSpellLevel > 0 && paladinSpellLevel < 10 && creature.ActiveEffects.Any(e => e.Tag == EffectSystem.ChampionAntiqueEffectTag && e.Creator == caster))
          {
            LogUtils.LogMessage("Désavantage - Champion Antique", LogUtils.LogType.Combat);
            return true;
          }

          if (spellEntry.RowIndex == CustomSpell.ConspuerLennemi && Utils.In(creature.Race.RacialType, RacialType.Undead, RacialType.Outsider))
          {
            LogUtils.LogMessage("Désavantage - Conspuer l'ennemi", LogUtils.LogType.Combat);
            return true;
          }

          if (spellEntry.RowIndex == CustomSpell.Fracassement && creature.Race.RacialType == RacialType.Construct)
          {
            LogUtils.LogMessage("Désavantage - Fracassement vs Créature Artificielle", LogUtils.LogType.Combat);
            return true;
          }

          if(caster.ActiveEffects.Any(e => e.Tag == EffectSystem.MetamagieEffectTag && e.IntParams[5] == CustomSkill.EnsoIntensification))
          {
            LogUtils.LogMessage("Désavantage - Métamagie : Intensification", LogUtils.LogType.Combat);
            EffectUtils.RemoveTaggedParamEffect(caster, CustomSkill.EnsoIntensification, EffectSystem.MetamagieEffectTag);
            return true;
          }
        }
      }

      foreach (var eff in creature.ActiveEffects)
      {
        if (spellEntry is not null && eff.Tag == EffectSystem.HaloDeLumiereEffectTag
          && (spellEntry.damageType.Contains(DamageType.Fire) || spellEntry.damageType.Contains(DamageType.Divine)))
        {
          LogUtils.LogMessage("Désavantage - Halo de Lumière", LogUtils.LogType.Combat);
          return true;
        }

        switch (eff.Tag)
        {
          case EffectSystem.EntraveEffectTag:

            if (ability == Ability.Dexterity)
            {
              LogUtils.LogMessage("Désavantage - Entravé", LogUtils.LogType.Combat);
              return true;
            }

            break;

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

          case EffectSystem.FrappeSideranteEffectTag:
            LogUtils.LogMessage("Désavantage - Frappe Sidérante", LogUtils.LogType.Combat);
            EffectUtils.RemoveTaggedEffect(creature, EffectSystem.FrappeSideranteEffectTag);
            return true;

          case EffectSystem.FrappeOcculteEffectTag:

            if (spellEntry is not null && oCaster is not null && eff.Creator == oCaster)
            {
              creature.RemoveEffect(eff);
              LogUtils.LogMessage("Désavantage - Chevalier Occulte : Frappe Occulte", LogUtils.LogType.Combat);
              return true;
            }

            break;
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

      switch(effectType)
      {
        case SpellEffectType.Concentration:

          if (oCaster is NwCreature attacker && attacker.KnowsFeat((Feat)CustomSkill.PourfendeurDeMages))
          {
            LogUtils.LogMessage("Désavantage - Concentration vs Pourfendeur de Mages", LogUtils.LogType.Combat);
            return true;
          }

          break;
      }

      return false;
    }
  }
}
