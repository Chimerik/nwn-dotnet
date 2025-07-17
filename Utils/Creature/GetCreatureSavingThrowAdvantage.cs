using Anvil.API;
using static NWN.Systems.SpellConfig;
using Ability = Anvil.API.Ability;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static int GetCreatureSavingThrowAdvantage(NwCreature creature, Ability ability, SpellEntry spellEntry = null, SpellEffectType effectType = SpellEffectType.Invalid, NwGameObject caster = null)
    {
      switch(ability)
      {
        case Ability.Strength:
        case Ability.Dexterity:

          foreach(var eff in creature.ActiveEffects)
            if (spellEntry is not null && SpellUtils.HandleSpellTargetIncapacitated(caster, creature, eff, spellEntry, NwSpell.FromSpellId(spellEntry.RowIndex).InnateSpellLevel))
              return -1000;

          break;
      }

      bool advantage = ComputeCreatureSavingThrowAdvantage(creature, ability, spellEntry, effectType, caster);
      bool disadvantage = ComputeCreatureSavingThrowDisadvantage(creature, ability, spellEntry, effectType, caster);

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
