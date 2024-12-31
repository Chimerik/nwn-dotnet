using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> BrandingSmite(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster)
        return new List<NwGameObject>();

      StringUtils.ForceBroadcastSpellCasting(caster, spell);
      EffectUtils.ClearChatimentEffects(caster);
      caster.Location.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.VisualEffect(VfxType.ImpPulseHoly), Effect.VisualEffect(VfxType.ImpDivineStrikeHoly)));

      NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.brandingSmiteAttack, SpellUtils.GetSpellDuration(oCaster, spellEntry)));

      caster.OnCreatureAttack -= CreatureUtils.OnAttackBrandingSmite;
      caster.OnCreatureAttack += CreatureUtils.OnAttackBrandingSmite;

      FeatUtils.DecrementFeatUses(caster, CustomSkill.BrandingSmite);

      return new List<NwGameObject>() { caster };
    }
  }
}
