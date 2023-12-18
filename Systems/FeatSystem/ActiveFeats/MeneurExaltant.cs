using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void MeneurExaltant(NwCreature caster)
    {
      foreach(var creature in caster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 6, false))
      {
        if (creature.HP < 1 || creature.IsReactionTypeHostile(caster) || creature.GetObjectVariable<LocalVariableInt>("_MENEUR_EXALTANT_BUFF").HasValue)
          continue;

        creature.ApplyEffect(EffectDuration.Permanent, Effect.TemporaryHitpoints(caster.Level + caster.GetAbilityModifier(Ability.Charisma)));
        creature.GetObjectVariable<LocalVariableInt>("_MENEUR_EXALTANT_BUFF").Value = 1;
      }

      caster.DecrementRemainingFeatUses(NwFeat.FromFeatId(CustomSkill.MeneurExaltant));
    }
  }
}
