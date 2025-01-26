using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void RestaurationPartielle(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);

      var type = spell.Id switch
      {
        CustomSpell.RestaurationAveuglement => EffectType.Blindness,
        CustomSpell.RestaurationParalysie => EffectType.Paralyze,
        CustomSpell.RestaurationSurdite => EffectType.Deaf,
        _ => EffectType.Poison,
      };

      foreach (var target in targets)
      {
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpRestorationLesser));

        EffectUtils.RemoveEffectType(target, type);

        if (type == EffectType.Poison)
          EffectUtils.RemoveTaggedEffect(target, EffectSystem.PoisonEffectTag);
      }
    }
  }
}
