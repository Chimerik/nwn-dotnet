using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> BladeWard(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpAcBonus));
      oCaster.ApplyEffect(EffectDuration.Temporary, EffectSystem.ProtectionContreLesLames, SpellUtils.GetSpellDuration(oCaster, spellEntry));

      return new List<NwGameObject>() { oCaster };
    }
  }
}
