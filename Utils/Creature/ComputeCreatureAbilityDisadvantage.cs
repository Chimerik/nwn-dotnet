using Anvil.API;
using static NWN.Systems.SpellConfig;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool ComputeCreatureAbilityDisadvantage(NwCreature creature, Ability ability, SpellEntry spellEntry = null, SpellEffectType effectType = SpellEffectType.Invalid, NwGameObject oCaster = null)
    {
      if (oCaster is NwCreature caster && caster.KnowsFeat((Feat)CustomSkill.ArcaneTricksterMagicalAmbush) && !creature.IsCreatureSeen(caster))
        return true;

        foreach (var eff in creature.ActiveEffects)
      {
        switch(eff.Tag)
        {
          case EffectSystem.FrightenedEffectTag: return true;
          case EffectSystem.FrappeOcculteEffectTag: 
            
            if(spellEntry is not null && oCaster is not null && eff.Creator == oCaster)
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
