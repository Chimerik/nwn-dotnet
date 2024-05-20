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
        return true;

      if (spellEntry is not null && creature.KnowsFeat((Feat)CustomSkill.AbjurationSpellResistance))
        return true;

      switch (ability)
      {
        case Ability.Strength:

          if (creature.KnowsFeat((Feat)CustomSkill.TotemAspectOurs))
            return true;

          break;

        case Ability.Dexterity:

          if (effectType == SpellEffectType.Stealth && creature.KnowsFeat((Feat)CustomSkill.ThiefDiscretionSupreme))
            return true;

          if (creature.Classes.Any(c => c.Class.ClassType == ClassType.Barbarian && c.Level > 1) && !creature.ActiveEffects.Any(e => e.EffectType == EffectType.Blindness || e.EffectType == EffectType.Deaf))
            return true;

          break;

        case Ability.Intelligence:
        case Ability.Wisdom:
        case Ability.Charisma:

          if (creature.Race.Id == CustomRace.RockGnome || creature.Race.Id == CustomRace.ForestGnome
            || creature.Race.Id == CustomRace.DeepGnome)
            return true;

          break;
      }
      
      switch(effectType)
      {
        case SpellEffectType.Poison:

          if(creature.Race.Id == CustomRace.GoldDwarf || creature.Race.Id == CustomRace.ShieldDwarf 
            || creature.Race.Id == CustomRace.StrongheartHalfling)
            return true;

          break;

        case SpellEffectType.Charm:

          if (creature.Race.Id == CustomRace.HighElf || creature.Race.Id == CustomRace.HighHalfElf 
            || creature.Race.Id == CustomRace.WoodElf || creature.Race.Id == CustomRace.WoodHalfElf
            || creature.Race.Id == CustomRace.Duergar)
            return true;

          break;

        case SpellEffectType.Fear:
        case SpellEffectType.Terror:

          if (creature.Race.Id == CustomRace.StrongheartHalfling || creature.Race.Id == CustomRace.LightfootHalfling
            || creature.KnowsFeat((Feat)CustomSkill.ChasseurMoralDacier))
            return true;

          break;

        case SpellEffectType.Paralysis:
        case SpellEffectType.Illusion:

          if (creature.Race.Id == CustomRace.Duergar)
            return true;

          break;

        case SpellEffectType.Knockdown:

          if (creature.KnowsFeat((Feat)CustomSkill.TotemAspectCrocodile))
            return true;

          break;
      }
      
      if (oCaster is NwCreature caster)
      {
        NwFeat favoredEnemyFeat = caster.Race.GetFavoredEnemyFeat();

        if(favoredEnemyFeat is not null && creature.KnowsFeat(favoredEnemyFeat) && creature.KnowsFeat((Feat)CustomSkill.RangerGreaterFavoredEnemy))
            return true;
        else if (creature.KnowsFeat((Feat)CustomSkill.TueurDeMage) && creature.DistanceSquared(caster) < 7)
          return true;
      }

      foreach (var eff in creature.ActiveEffects)
      {
        switch (ability)
        {
          case Ability.Strength:

            switch (eff.Tag)
            {
              case EffectSystem.EnlargeEffectTag:
              case EffectSystem.RageDuSanglierEffectTag:
              case EffectSystem.BarbarianRageEffectTag: return true;
            }

            break;

          case Ability.Dexterity:

            if (EffectSystem.DodgeEffectTag == eff.Tag)
              return true;

            if (EffectSystem.MonkPatienceEffectTag == eff.Tag)
              return true;

            break;
        }

        switch(effectType)
        {
          case SpellEffectType.Charm:
          case SpellEffectType.Fear:
            if (EffectSystem.ContreCharmeEffectTag == eff.Tag)
              return true;

            break;
        }
      }

      return false;
    }
  }
}
