using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> ProtectionContreLeMalEtLeBien(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);

      foreach (var target in targets)
      {
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(spellEntry.damageVFX));
        target.ApplyEffect(EffectDuration.Temporary, EffectSystem.ProtectionContreLeMalEtLeBien, SpellUtils.GetSpellDuration(oCaster, spellEntry));
      }
      return targets;
    }
  }
}
