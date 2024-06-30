using System.Linq;
using Anvil.API;
using static NWN.Systems.SpellConfig;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool ComputeCreatureAbilityAdvantage(NwCreature creature, Ability ability, SpellEntry spellEntry = null, SpellEffectType effectType = SpellEffectType.Invalid, NwGameObject oCaster = null)
    {
      if (creature.KnowsFeat((Feat)CustomSkill.BelluaireDefenseDeLaBete) && !creature.IsLoginPlayerCharacter)
      {
        LogUtils.LogMessage("Avantage - Belluaire : Défense de la Bête", LogUtils.LogType.Combat);
        return true;
      }

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

      switch (ability)
      {
        case Ability.Strength:

          if (creature.KnowsFeat((Feat)CustomSkill.TotemAspectOurs))
          {
            LogUtils.LogMessage("Avantage - Aspect de l'Ours", LogUtils.LogType.Combat);
            return true;
          }

          break;

        case Ability.Dexterity:

          if (effectType == SpellEffectType.Stealth)
          {
            if (creature.KnowsFeat((Feat)CustomSkill.ThiefDiscretionSupreme))
            {
              LogUtils.LogMessage("Avantage - Voleur : Discrétion Suprème", LogUtils.LogType.Combat);
              return true;
            }

            if(creature.ActiveEffects.Any(e => e.Tag == EffectSystem.BenedictionEscrocEffectTag))
            {
              LogUtils.LogMessage("Avantage - Bénédiction de l'Escroc", LogUtils.LogType.Combat);
              return true;
            }
          }

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

            if (effectType == SpellEffectType.Stealth && oCaster.ActiveEffects.Any(e => e.Tag == EffectSystem.MarqueDuChasseurTag && e.Creator == creature))
            {
              LogUtils.LogMessage("Avantage - Perception vs cible sous Marque du Chasseur", LogUtils.LogType.Combat);
              return true;
            }

            if (creature.ActiveEffects.Any(e => e.Tag == EffectSystem.LueurDespoirEffectTag))
            {
              LogUtils.LogMessage("Avantage - Lueur d'espoir vs jet sagesse", LogUtils.LogType.Combat);
              return true;
            }
          }

          break;
        }

      switch (effectType)
      {
        case SpellEffectType.Poison:

          if (creature.Race.Id == CustomRace.GoldDwarf || creature.Race.Id == CustomRace.ShieldDwarf
            || creature.Race.Id == CustomRace.StrongheartHalfling)
          {
            LogUtils.LogMessage("Avantage - Nain ou Coeur-Vaillant contre poison", LogUtils.LogType.Combat);
            return true;
          }

          break;

        case SpellEffectType.Charm:

          if (creature.Race.Id == CustomRace.HighElf || creature.Race.Id == CustomRace.HighHalfElf
            || creature.Race.Id == CustomRace.WoodElf || creature.Race.Id == CustomRace.WoodHalfElf
            || creature.Race.Id == CustomRace.Duergar)
          {
            LogUtils.LogMessage("Avantage - Elfe, demi-elfe ou duergar contre charme", LogUtils.LogType.Combat);
            return true;
          }

          break;

        case SpellEffectType.Fear:
        case SpellEffectType.Terror:

          if (creature.Race.Id == CustomRace.StrongheartHalfling || creature.Race.Id == CustomRace.LightfootHalfling
            || creature.KnowsFeat((Feat)CustomSkill.ChasseurMoralDacier))
          {
            LogUtils.LogMessage("Avantage - Halfelin ou moral d'acier de Chasser vs effroi", LogUtils.LogType.Combat);
            return true;

          }

          break;

        case SpellEffectType.Paralysis:
        case SpellEffectType.Illusion:

          if (creature.Race.Id == CustomRace.Duergar)
          {
            LogUtils.LogMessage("Avantage - Duergar vs Paralysie ou Illusion", LogUtils.LogType.Combat);
            return true;
          }

          break;

        case SpellEffectType.Knockdown:

          if (creature.KnowsFeat((Feat)CustomSkill.TotemAspectCrocodile))
          {
            LogUtils.LogMessage("Avantage - Barbare Aspect du Crocodile contre Renversement", LogUtils.LogType.Combat);
            return true;
          }

          break;

        case SpellEffectType.Death:

          if (creature.ActiveEffects.Any(e => e.Tag == EffectSystem.LueurDespoirEffectTag))
          {
            LogUtils.LogMessage("Avantage - Lueur d'espoir vs jet contre la mort", LogUtils.LogType.Combat);
            return true;
          }

          break;
      }

      if (oCaster is NwCreature caster)
      {
        NwFeat favoredEnemyFeat = caster.Race.GetFavoredEnemyFeat();

        if (favoredEnemyFeat is not null && creature.KnowsFeat(favoredEnemyFeat) && creature.KnowsFeat((Feat)CustomSkill.RangerGreaterFavoredEnemy))
        {
          LogUtils.LogMessage("Avantage - Rôdeur vs ennemi juré", LogUtils.LogType.Combat);
          return true;
        }
        else if (creature.KnowsFeat((Feat)CustomSkill.TueurDeMage) && creature.DistanceSquared(caster) < 7)
        {
          LogUtils.LogMessage("Avantage - Tueur de mage", LogUtils.LogType.Combat);
          return true;
        }

        if (spellEntry is not null && Utils.In(caster.Race.RacialType, RacialType.Undead, RacialType.Outsider) && creature.ActiveEffects.Any(e => e.Tag == EffectSystem.NimbeSacreeAuraEffectTag))
        {
          LogUtils.LogMessage("Avantage - Nimbe Sacrée : Résistance aux sorts des extérieurs et mort-vivants", LogUtils.LogType.Combat);
          return true;
        }
      }

      foreach (var eff in creature.ActiveEffects)
      {
        switch (ability)
        {
          case Ability.Strength:

            switch (eff.Tag)
            {
              case EffectSystem.EnlargeEffectTag: LogUtils.LogMessage("Avantage - Agrandissement", LogUtils.LogType.Combat); return true;
              case EffectSystem.RageDuSanglierEffectTag:
              case EffectSystem.BarbarianRageEffectTag:
                LogUtils.LogMessage("Avantage - Barbare : Rage", LogUtils.LogType.Combat);
                return true;
            }

            break;

          case Ability.Dexterity:

            if (EffectSystem.DodgeEffectTag == eff.Tag)
            {
              LogUtils.LogMessage("Avantage - Mode Esquive", LogUtils.LogType.Combat);
              return true;
            }

            if (EffectSystem.MonkPatienceEffectTag == eff.Tag)
            {
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
        }
      }

      return false;
    }
  }
}
