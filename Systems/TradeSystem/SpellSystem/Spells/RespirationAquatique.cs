
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void RespirationAquatique(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      foreach (NwCreature target in oCaster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false))
      {
        if (!caster.IsReactionTypeHostile(target))
          target.ApplyEffect(EffectDuration.Temporary, EffectSystem.RespirationAquatique, NwTimeSpan.FromRounds(spellEntry.duration));
      }

      oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPulseWater));
    }
  }
}
