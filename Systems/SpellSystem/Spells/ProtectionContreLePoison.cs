using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void ProtectionContreLePoison(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpRemoveCondition));
      oTarget.ApplyEffect(EffectDuration.Temporary, EffectSystem.ProtectionContreLePoison, SpellUtils.GetSpellDuration(oCaster, spellEntry));
    }
  }
}
