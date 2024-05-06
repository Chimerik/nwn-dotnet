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

      foreach (var creature in caster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 18, false))
      {
        if (creature == caster)
          continue;

        foreach(var eff in creature.ActiveEffects)
          if(eff.Tag == EffectSystem.ResonanceKiEffectTag && eff.Creator == caster)
          {
            foreach(var target in creature.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 5, false))
            {
              SpellConfig.SavingThrowFeedback feedback = new();
              int advantage = CreatureUtils.GetCreatureAbilityAdvantage(target, Ability.Dexterity);
              int totalSave = SpellUtils.GetSavingThrowRoll(target, Ability.Dexterity, DC, advantage, feedback);
              bool saveFailed = totalSave < DC;

              SpellUtils.SendSavingThrowFeedbackMessage(creature, target, feedback, advantage, DC, totalSave, saveFailed, Ability.Dexterity);

              int damage = NwRandom.Roll(Utils.random, 6, 3);

              if (saveFailed)
                damage /= 2;

              target.ApplyEffect(EffectDuration.Instant, Effect.Damage(damage));
            }

            creature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSoundBurst));

            caster.RemoveEffect(eff);

            break;
          }
      }

      FeatUtils.DecrementKi(caster);
    }
  }
}
