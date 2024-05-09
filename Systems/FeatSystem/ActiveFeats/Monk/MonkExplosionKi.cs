using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void MonkExplosionKi(NwCreature caster)
    {
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Explosion Ki", StringUtils.gold, true, true);

      int attackerModifier = caster.GetAbilityModifier(Ability.Wisdom);
      int DC = 8 + NativeUtils.GetCreatureProficiencyBonus(caster) + attackerModifier;

      List<NwCreature> alreadyHitList = new();

      foreach (var creature in caster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 18, false))
      {
        if (creature == caster || alreadyHitList.Contains(creature))
          continue;

        foreach (var eff in creature.ActiveEffects)
          if(eff.Tag == EffectSystem.ResonanceKiEffectTag && eff.Creator == caster)
          {
            creature.RemoveEffect(eff);
            creature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSoundBurst));
            int damage = NwRandom.Roll(Utils.random, 6, 3);

            if (!CreatureUtils.GetSavingThrow(caster, creature, Ability.Dexterity, DC))
              damage /= 2;

            creature.ApplyEffect(EffectDuration.Instant, Effect.Damage(damage));
            alreadyHitList.Add(creature);

            foreach (var target in creature.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 5, false))
            {
              if(alreadyHitList.Contains(target)) 
                continue; 
              
              damage = NwRandom.Roll(Utils.random, 6, 3);

              if (!CreatureUtils.GetSavingThrow(caster, target, Ability.Dexterity, DC))
                damage /= 2;

              target.ApplyEffect(EffectDuration.Instant, Effect.Damage(damage));
              alreadyHitList.Add(target);
            }

            break;
          }
      }

      FeatUtils.DecrementKi(caster);
    }
  }
}
