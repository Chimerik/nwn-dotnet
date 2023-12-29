using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using NWN.Systems;
using static NWN.Systems.SpellConfig;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static int GetCreatureAbilityAdvantage(NwCreature creature, Ability ability, SpellEntry spellEntry = null, SpellEffectType effectType = SpellEffectType.Invalid, NwCreature caster = null)
    {
      Dictionary<string, bool> disadvantageDictionary = new()
      {
        { EffectSystem.ShieldArmorDisadvantageEffectTag, false } ,
      };

      Dictionary<string, bool> advantageDictionary = new()
      {
        { EffectSystem.EnlargeEffectTag, false },
        { EffectSystem.DodgeEffectTag, false },
      };

      int advantage = 0;

      foreach (var eff in creature.ActiveEffects)
      {
        switch(ability)
        {
          case Ability.Strength:

            if (spellEntry is not null && SpellUtils.HandleSpellTargetIncapacitated(caster, creature, eff, spellEntry))
              return -1000;

            advantageDictionary[EffectSystem.EnlargeEffectTag] = advantageDictionary[EffectSystem.EnlargeEffectTag] || EffectSystem.EnlargeEffectTag == eff.Tag;

            disadvantageDictionary[EffectSystem.ShieldArmorDisadvantageEffectTag] = disadvantageDictionary[EffectSystem.ShieldArmorDisadvantageEffectTag] || EffectSystem.ShieldArmorDisadvantageEffectTag == eff.Tag;
            break;

          case Ability.Dexterity:

            if (spellEntry is not null && SpellUtils.HandleSpellTargetIncapacitated(caster, creature, eff, spellEntry))
              return -1000;

            advantageDictionary[EffectSystem.DodgeEffectTag] = advantageDictionary[EffectSystem.DodgeEffectTag] || EffectSystem.DodgeEffectTag == eff.Tag;

            disadvantageDictionary[EffectSystem.ShieldArmorDisadvantageEffectTag] = disadvantageDictionary[EffectSystem.ShieldArmorDisadvantageEffectTag] || EffectSystem.ShieldArmorDisadvantageEffectTag == eff.Tag;
            break;
        }
      }

      switch(ability)
      {
        case Ability.Intelligence:
        case Ability.Wisdom:
        case Ability.Charisma:

          if (creature.Race.Id == CustomRace.RockGnome || creature.Race.Id == CustomRace.ForestGnome
            || creature.Race.Id == CustomRace.DeepGnome)
            advantage += 1;

          break;
      }
      
      switch(effectType)
      {
        case SpellEffectType.Poison:

          if(creature.Race.Id == CustomRace.GoldDwarf || creature.Race.Id == CustomRace.ShieldDwarf 
            || creature.Race.Id == CustomRace.StrongheartHalfling)
            advantage += 1;

          break;

        case SpellEffectType.Charm:

          if (creature.Race.Id == CustomRace.HighElf || creature.Race.Id == CustomRace.HighHalfElf 
            || creature.Race.Id == CustomRace.WoodElf || creature.Race.Id == CustomRace.WoodHalfElf
            || creature.Race.Id == CustomRace.Duergar)
            advantage += 1;

          break;

        case SpellEffectType.Fear:
        case SpellEffectType.Terror:

          if (creature.Race.Id == CustomRace.StrongheartHalfling || creature.Race.Id == CustomRace.LightfootHalfling)
            advantage += 1;

          break;

        case SpellEffectType.Paralysis:
        case SpellEffectType.Illusion:

          if (creature.Race.Id == CustomRace.Duergar)
            advantage += 1;

          break;
      }

      if(caster is not null && creature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.TueurDeMage))
        && creature.DistanceSquared(caster) < 7)
        advantage += 1;

      return -disadvantageDictionary.Count(v => v.Value) + advantageDictionary.Count(v => v.Value) + advantage;
    }
  }
}
