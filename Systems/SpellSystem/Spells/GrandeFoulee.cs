using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void GrandeFoulee(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      EffectUtils.RemoveTaggedEffect(oTarget, EffectSystem.GrandeFouleeEffectTag);

      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHaste));
      oTarget.ApplyEffect(EffectDuration.Temporary, EffectSystem.GrandeFoulee, SpellUtils.GetSpellDuration(oCaster, spellEntry));
    }  
  }
}
