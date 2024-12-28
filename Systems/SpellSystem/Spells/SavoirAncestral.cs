using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void SavoirAncestral(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if(oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpImproveAbilityScore));
      NWScript.AssignCommand(oCaster, () => oCaster.ApplyEffect(EffectDuration.Temporary, EffectSystem.SavoirAncestral(spellEntry.savingThrowAbility), NwTimeSpan.FromRounds(spellEntry.duration)));

      caster.IncrementRemainingFeatUses((Feat)CustomSkill.ClercSavoirAncestral);
      ClercUtils.ConsumeConduitDivin(caster);
    }
  }
}
