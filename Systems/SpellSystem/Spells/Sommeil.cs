using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Sommeil(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      int DV = NwRandom.Roll(Utils.random, 8, 5);

      foreach(var target in targetLocation.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false))
      {
        if (DV < 1)
          break;

        if (target.HP < 1 || target.Level > DV || EffectSystem.IsCharmeImmune(caster, target)
          || target.ActiveEffects.Any(e => e.EffectType == EffectType.Sleep))
          continue;

        target.ApplyEffect(EffectDuration.Temporary, Effect.Sleep(), NwTimeSpan.FromRounds(spellEntry.duration));
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSleep));

        DV -= target.Level;
      }
    }
  }
}
