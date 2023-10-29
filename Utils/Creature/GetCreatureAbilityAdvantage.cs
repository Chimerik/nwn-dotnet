using Anvil.API;
using NWN.Systems;
using static NWN.Systems.SpellConfig;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static int GetCreatureAbilityAdvantage(NwCreature creature, SpellEntry spellEntry, SpellEffectType effectType = SpellEffectType.Invalid, NwCreature caster = null)
    {
      Ability ability = spellEntry.savingThrowAbility;
      int advantage = 0;

      foreach (var eff in creature.ActiveEffects)
      {
        switch(ability)
        {
          case Ability.Strength:
          case Ability.Dexterity:

            if (SpellUtils.HandleSpellTargetIncapacitated(caster, creature, eff.EffectType, spellEntry))
              return -1000;

            break;
        }  
          

        advantage += GetAbilityAdvantageFromEffect(ability, eff.Tag);
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

          return advantage;

        case SpellEffectType.Charm:

          if (creature.Race.Id == CustomRace.HighElf || creature.Race.Id == CustomRace.HighHalfElf 
            || creature.Race.Id == CustomRace.WoodElf || creature.Race.Id == CustomRace.WoodHalfElf
            || creature.Race.Id == CustomRace.Duergar)
            advantage += 1;

          return advantage;

        case SpellEffectType.Fear:
        case SpellEffectType.Terror:

          if (creature.Race.Id == CustomRace.StrongheartHalfling || creature.Race.Id == CustomRace.LightfootHalfling)
            advantage += 1;

          return advantage;

        case SpellEffectType.Paralysis:
        case SpellEffectType.Illusion:

          if (creature.Race.Id == CustomRace.Duergar)
            advantage += 1;

          return advantage;

      }

      return advantage;
    }
  }
}
