using Anvil.API;
using static NWN.Systems.SpellConfig;
using Ability = Anvil.API.Ability;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static int GetCreatureAbilityAdvantage(NwCreature creature, Ability ability, SpellEntry spellEntry = null, SpellEffectType effectType = SpellEffectType.Invalid, NwCreature caster = null, byte spellLevel = 0)
    {
      switch(ability)
      {
        case Ability.Strength:
        case Ability.Dexterity:

          foreach(var eff in creature.ActiveEffects)
            if (spellEntry is not null && SpellUtils.HandleSpellTargetIncapacitated(caster, creature, eff, spellEntry, spellLevel))
              return -1000;

          break;
      }

      bool advantage = ComputeCreatureAbilityAdvantage(creature, ability, spellEntry, effectType, caster);
      bool disadvantage = ComputeCreatureAbilityDisadvantage(creature, ability, spellEntry, effectType, caster);

      if (advantage)
      {
        if (disadvantage)
          return 0;

        return 1;
      }

      if (disadvantage)
        return -1;

      return 0;
    }
  }
}
