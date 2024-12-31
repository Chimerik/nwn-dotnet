using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void ArmureDeMage(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      EffectUtils.RemoveTaggedEffect(oTarget, EffectSystem.ArmureDeMageEffectTag);

      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpAcBonus));
      oTarget.ApplyEffect(EffectDuration.Temporary, EffectSystem.ArmureDeMage, SpellUtils.GetSpellDuration(oCaster, spellEntry));
    }  
  }
}
