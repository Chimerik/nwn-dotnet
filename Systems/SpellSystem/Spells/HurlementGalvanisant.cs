using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void HurlementGalvanisant(NwCreature oCaster, SpellEvents.OnSpellCast onSpellCast, SpellEntry spellEntry)
    {
      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, oCaster, onSpellCast.Spell.SpellType);

      if(oCaster.Gender == Gender.Male)
        oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfHowlWarCry));
      else
        oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfHowlWarCryFemale));

      foreach (NwCreature target in onSpellCast.TargetLocation.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 9, false, oCaster.Location.Position))
      {
        if (target.IsReactionTypeHostile(oCaster))
          continue;

        target.ApplyEffect(EffectDuration.Temporary, EffectSystem.hurlementGalvanisant, NwTimeSpan.FromRounds(spellEntry.duration));
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadSonic));
      }
    }
  }
}
