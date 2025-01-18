using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void BrandingSmite(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwFeat feat)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      EffectUtils.ClearChatimentEffects(caster);
      caster.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.VisualEffect(VfxType.ImpPulseHoly), Effect.VisualEffect(VfxType.ImpDivineStrikeHoly)));

      NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.brandingSmiteAttack));

      caster.OnCreatureAttack -= CreatureUtils.OnAttackBrandingSmite;
      caster.OnCreatureAttack += CreatureUtils.OnAttackBrandingSmite;

      if (feat is not null)
        FeatUtils.DecrementFeatUses(caster, CustomSkill.BrandingSmite);
    }
  }
}
