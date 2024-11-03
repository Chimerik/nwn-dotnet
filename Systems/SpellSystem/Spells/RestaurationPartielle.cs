using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void RestaurationPartielle(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass castingClass)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);

      foreach (var target in targets)
      {
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpRestorationLesser));

        if (!EffectUtils.RemoveFirstEffectType(target, EffectType.Paralyze))
          if (!EffectUtils.RemoveFirstEffectType(target, EffectType.Blindness))
            if (!EffectUtils.RemoveFirstEffectType(target, EffectType.Poison))
              if (!EffectUtils.RemoveFirstEffectType(target, EffectType.Deaf))
                EffectUtils.RemoveFirstEffectType(target, EffectType.Disease);
      }
    }
  }
}
