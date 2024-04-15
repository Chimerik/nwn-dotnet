using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void PassageSansTrace(SpellEvents.OnSpellCast onSpellCast, SpellEntry spellEntry)
    {
      if (onSpellCast.Caster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, caster, onSpellCast.Spell.SpellType);

      foreach (NwCreature target in onSpellCast.TargetLocation.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 9, false))
      {
        if (target.IsEnemy(caster))
          continue;

        target.ApplyEffect(EffectDuration.Temporary, Effect.SkillIncrease(NwSkill.FromSkillType(Skill.MoveSilently), 10));
      }
    }
  }
}
