using Anvil.API;
using static NWN.Systems.SpellConfig;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool ComputeCreatureAbilityDisadvantage(NwCreature creature, Ability ability, SpellEntry spellEntry = null, SpellEffectType effectType = SpellEffectType.Invalid, NwGameObject caster = null)
    {     
      foreach (var eff in creature.ActiveEffects)
      {
        switch(eff.Tag)
        {
          case EffectSystem.FrightenedEffectTag: return true;
          case EffectSystem.FrappeOcculteEffectTag: 
            
            if(spellEntry is not null && caster is not null && eff.Creator == caster)
              return true;

            break;
        }

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
