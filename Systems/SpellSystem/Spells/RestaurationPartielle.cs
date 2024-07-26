using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void RestaurationPartielle(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass castingClass)
    {
      if (oTarget is not NwCreature target)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpRestorationLesser));

      if (!EffectUtils.RemoveFirstEffectType(target, EffectType.Paralyze))
        if (!EffectUtils.RemoveFirstEffectType(target, EffectType.Blindness))
          if (!EffectUtils.RemoveFirstEffectType(target, EffectType.Poison))
            if (!EffectUtils.RemoveFirstEffectType(target, EffectType.Deaf))
              EffectUtils.RemoveFirstEffectType(target, EffectType.Disease);
    }  
  }
}
