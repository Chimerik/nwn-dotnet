using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void BrandingSmite(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster)
        return;

      StringUtils.ForceBroadcastSpellCasting(caster, spell);
      caster.Location.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.VisualEffect(VfxType.ImpPulseHoly), Effect.VisualEffect(VfxType.ImpDivineStrikeHoly)));

      NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.brandingSmiteAttack, NwTimeSpan.FromRounds(spellEntry.duration)));
      EffectSystem.ApplyConcentrationEffect(caster, spell.Id, new List<NwGameObject> { caster }, spellEntry.duration);

      caster.OnCreatureAttack -= CreatureUtils.OnAttackBrandingSmite;
      caster.OnCreatureAttack += CreatureUtils.OnAttackBrandingSmite;

      FeatUtils.DecrementFeatUses(caster, CustomSkill.BrandingSmite);
      CreatureUtils.HandleBonusActionCooldown(caster);
    }
  }
}
