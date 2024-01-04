using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void MeneurExaltant(NwCreature caster)
    {
      foreach(var creature in caster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 6, false))
      {
        if (creature.HP < 1 || creature.IsReactionTypeHostile(caster) || creature.GetObjectVariable<PersistentVariableInt>(CreatureUtils.MeneurExaltantVariable).HasValue)
          continue;

        creature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHolyAid));
        creature.ApplyEffect(EffectDuration.Permanent, Effect.TemporaryHitpoints(caster.Level + caster.GetAbilityModifier(Ability.Charisma)));
        creature.GetObjectVariable<PersistentVariableInt>(CreatureUtils.MeneurExaltantVariable).Value = 1;
      }

      FeatUtils.DecrementFeatUses(caster, CustomSkill.MeneurExaltant);
    }
  }
}
