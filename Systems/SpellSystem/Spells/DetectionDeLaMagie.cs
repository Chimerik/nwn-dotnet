using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> DetectionDeLaMagie(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpMagicalVision));
      oCaster.ApplyEffect(EffectDuration.Temporary, Effect.VisualEffect(VfxType.DurMagicalSight), SpellUtils.GetSpellDuration(oCaster, spellEntry));

      return new List<NwGameObject>() { oCaster };
    }
  }
}
