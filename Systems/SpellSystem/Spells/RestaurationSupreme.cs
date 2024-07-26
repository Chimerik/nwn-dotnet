using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void RestaurationSupreme(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass castingClass)
    {
      if (oTarget is not NwCreature target)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpRestorationGreater));

      EffectUtils.RemoveEffectType(target, EffectType.Charmed, EffectType.Petrify, EffectType.Curse, EffectType.AbilityDecrease);
      EffectUtils.RemoveTaggedEffect(target, EffectSystem.CharmEffectTag, EffectSystem.MaledictionAttaqueEffectTag, EffectSystem.MaledictionDegatsEffectTag, EffectSystem.MaledictionConstitutionEffectTag, EffectSystem.MaledictionDexteriteEffectTag, EffectSystem.MaledictionEffroiEffectTag, EffectSystem.MaledictionForceEffectTag, EffectSystem.MaledictionIntelligenceEffectTag, EffectSystem.MaledictionSagesseEffectTag);
    }  
  }
}
