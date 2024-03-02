using Anvil.API;
using NWN.Systems;
using static NWN.Systems.SpellConfig;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static bool ComputeCreatureAbilityDisadvantage(NwCreature creature, Ability ability, SpellEntry spellEntry = null, SpellEffectType effectType = SpellEffectType.Invalid, NwCreature caster = null)
    {     
      foreach (var eff in creature.ActiveEffects)
      {
        if (EffectSystem.FrightenedEffectTag == eff.Tag)
          return true;

        switch (ability)
        {
          case Ability.Strength:

            if (EffectSystem.ShieldArmorDisadvantageEffectTag == eff.Tag)
              return true;

            break;

          case Ability.Dexterity:

            if (EffectSystem.ShieldArmorDisadvantageEffectTag == eff.Tag)
              return true;

            break;

          case Ability.Constitution:

            if (EffectSystem.SaignementEffectTag == eff.Tag)
              return true;

            break;
        }
      }

      return false;
    }
  }
}
