using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void PassageSansTrace(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(caster, caster, spell.SpellType);

      foreach (NwCreature target in caster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 9, false))
      {
        if (target.IsEnemy(caster))
          continue;

        target.ApplyEffect(EffectDuration.Temporary, Effect.SkillIncrease(NwSkill.FromSkillType(Skill.MoveSilently), 10));
      }
    }
  }
}
