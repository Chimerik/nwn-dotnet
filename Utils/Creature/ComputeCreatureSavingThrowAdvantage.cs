using System.Linq;
using Anvil.API;
using static NWN.Systems.SpellConfig;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool ComputeCreatureSavingThrowAdvantage(NwCreature creature, Ability ability, SpellEntry spellEntry = null, SpellEffectType effectType = SpellEffectType.Invalid, NwGameObject oCaster = null)
    {
      if (spellEntry is not null && creature.KnowsFeat((Feat)CustomSkill.AbjurationSpellResistance))
      {
        LogUtils.LogMessage("Avantage - Abjuration : Résistance aux sorts", LogUtils.LogType.Combat);
        return true;
      }

      if (effectType == SpellEffectType.Trap && creature.KnowsFeat(Feat.KeenSense))
      {
        LogUtils.LogMessage("Avantage - Expert en donjons", LogUtils.LogType.Combat);
        return true;
      }
      
      if (effectType == SpellEffectType.Tasha) // Signifie qu'il s'agit du JDS de fou rire de Tasha après que la cible ait subit des dégâts
        return true;

      switch (ability)
      {
        case Ability.Dexterity:

          if ((spellEntry is not null || effectType == SpellEffectType.Trap) &&
            creature.Classes.Any(c => c.Class.ClassType == ClassType.Barbarian && c.Level > 1) && !creature.ActiveEffects.Any(e => e.EffectType == EffectType.Blindness || e.EffectType == EffectType.Deaf))
          {
            LogUtils.LogMessage("Avantage - Barbare : Sens du danger", LogUtils.LogType.Combat);
            return true;
          }

          break;

        case Ability.Intelligence:
        case Ability.Charisma:

          if (spellEntry is not null && (creature.Race.Id == CustomRace.RockGnome || creature.Race.Id == CustomRace.ForestGnome
            || creature.Race.Id == CustomRace.DeepGnome))
          {
            LogUtils.LogMessage("Avantage - Gnome vs jet mental", LogUtils.LogType.Combat);
            return true;
          }

          break;

        case Ability.Wisdom:

          if (spellEntry is not null)
          {
            if (creature.Race.Id == CustomRace.RockGnome || creature.Race.Id == CustomRace.ForestGnome
              || creature.Race.Id == CustomRace.DeepGnome)
            {
              LogUtils.LogMessage("Avantage - Gnome vs jet mental", LogUtils.LogType.Combat);
              return true;
            }

            if (spellEntry.RowIndex == (int)Spell.CharmPerson && oCaster is NwCreature charmer && creature.IsReactionTypeHostile(charmer))
            {
              LogUtils.LogMessage("Avantage - Charme-Personne vs cible hostile", LogUtils.LogType.Combat);
              return true;
            }
          }

          break;
        }

      switch (effectType)
      {
        case SpellEffectType.Concentration:

          if(creature.KnowsFeat((Feat)CustomSkill.MageDeGuerre))
          {
            LogUtils.LogMessage("Avantage - Concentration : Mage de Guerre", LogUtils.LogType.Combat);
            return true;
          }

          if (creature.KnowsFeat((Feat)CustomSkill.EspritOcculte))
          {
            LogUtils.LogMessage("Avantage - Concentration : Esprit Occulte", LogUtils.LogType.Combat);
            return true;
          }

          break;

        case SpellEffectType.Poison:

          if (creature.Race.Id == CustomRace.GoldDwarf)
          {
            LogUtils.LogMessage("Avantage - Nain ou Coeur-Vaillant contre poison", LogUtils.LogType.Combat);
            return true;
          }

          break;

        case SpellEffectType.Charm:

          if (Utils.In(creature.Race.Id, CustomRace.HighElf, CustomRace.HighHalfElf, CustomRace.WoodElf, CustomRace.Duergar, CustomRace.WoodHalfElf))
          {
            LogUtils.LogMessage("Avantage - Elfe, demi-elfe ou duergar contre charme", LogUtils.LogType.Combat);
            return true;
          }

          break;

        case SpellEffectType.Fear:
        case SpellEffectType.Terror: break;

        case SpellEffectType.Paralysis:
        case SpellEffectType.Illusion:

          if (creature.Race.Id == CustomRace.Duergar)
          {
            LogUtils.LogMessage("Avantage - Duergar vs Paralysie ou Illusion", LogUtils.LogType.Combat);
            return true;
          }

          break; 
      }

      if (oCaster is NwCreature caster)
      {
        if (creature.KnowsFeat((Feat)CustomSkill.PourfendeurDeMages) && creature.DistanceSquared(caster) < 7)
        {
          LogUtils.LogMessage("Avantage - Pourfendeur de Mages", LogUtils.LogType.Combat);
          return true;
        }

        if (spellEntry is not null && Utils.In(caster.Race.RacialType, RacialType.Undead, CustomRacialType.Fielon) && creature.ActiveEffects.Any(e => e.Tag == EffectSystem.NimbeSacreeAuraEffectTag))
        {
          LogUtils.LogMessage("Avantage - Nimbe Sacrée : Résistance aux sorts des extérieurs et mort-vivants", LogUtils.LogType.Combat);
          return true;
        }
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
            }

            break;

          case Ability.Dexterity:

            switch(eff.Tag)
            {
              case EffectSystem.DodgeEffectTag:
                LogUtils.LogMessage("Avantage - Mode Esquive", LogUtils.LogType.Combat);
                return true;

              case EffectSystem.MonkPatienceEffectTag:
                LogUtils.LogMessage("Avantage - Moine : Patience", LogUtils.LogType.Combat);
                return true;
            }

            break;

          case Ability.Wisdom:

            switch (eff.Tag)
            {
              case EffectSystem.LueurDespoirEffectTag:
                LogUtils.LogMessage("Avantage - Lueur d'espoir vs jet sagesse", LogUtils.LogType.Combat);
                return true;

              case EffectSystem.MonkPatienceEffectTag:
                LogUtils.LogMessage("Avantage - Moine : Patience", LogUtils.LogType.Combat);
                return true;
            }

            break;
        }

        switch(effectType)
        {
          case SpellEffectType.Charm:
          case SpellEffectType.Fear:
            if (EffectSystem.ContreCharmeEffectTag == eff.Tag)
            {
              LogUtils.LogMessage("Avantage - Barde : Contre-charme", LogUtils.LogType.Combat);
              return true;
            }

            break;

          case SpellEffectType.Concentration:

            if (EffectSystem.ConcentrationAdvantageEffectTag == eff.Tag)
            {
              LogUtils.LogMessage("Avantage - Concentration", LogUtils.LogType.Combat);
              return true;
            }

            break;

          case SpellEffectType.Death:

            if (EffectSystem.LueurDespoirEffectTag == eff.Tag)
            {
              LogUtils.LogMessage("Avantage - Lueur d'espoir vs jet contre la mort", LogUtils.LogType.Combat);
              return true;
            }

            break;

          case SpellEffectType.Poison:

            switch(eff.Tag)
            {
              case EffectSystem.ConstitutionInfernaleEffectTag:
                LogUtils.LogMessage("Avantage - Constitution Infernale", LogUtils.LogType.Combat);
                return true;

              case EffectSystem.ProtectionContreLePoisonEffectTag:
                LogUtils.LogMessage("Avantage - Protection contre le Poison", LogUtils.LogType.Combat);
                return true;
            }

            break;
        }
      }

      return false;
    }
  }
}
