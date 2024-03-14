using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void PassageSansTrace(NwCreature caster, SpellEvents.OnSpellCast onSpellCast, SpellEntry spellEntry)
    {
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
