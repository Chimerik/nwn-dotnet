﻿using Anvil.API;

namespace NWN.Systems
{
  public static partial class OccultisteUtils
  {
    public static void HandleResilienceCeleste(NwCreature caster)
    {
      if (!caster.KnowsFeat((Feat)CustomSkill.ResilienceCeleste))
        return;

      int occultisteLevel = caster.GetClassInfo((ClassType)CustomClass.Occultiste).Level;
      int chaMod = CreatureUtils.GetAbilityModifierMin1(caster, Ability.Charisma);
      int casterBuff = occultisteLevel + chaMod;
      int allyBuff = (occultisteLevel / 2) + chaMod;

      caster.ApplyEffect(EffectDuration.Permanent, Effect.TemporaryHitpoints(casterBuff));
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHolyAid));

      foreach(var target in caster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 3, false))
      {
        if(target.HP < 1 || caster.IsReactionTypeHostile(target)) 
          continue;

        target.ApplyEffect(EffectDuration.Permanent, Effect.TemporaryHitpoints(allyBuff));
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHolyAid));
      }
    }
  }
}
