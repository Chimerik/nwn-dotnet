using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> SearingSmite(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster)
        return new List<NwGameObject>();

      StringUtils.ForceBroadcastSpellCasting(caster, spell);
      caster.Location.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.VisualEffect(VfxType.ImpPulseFire), Effect.VisualEffect(VfxType.ImpFlameM)));

      NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.searingSmiteAttack, NwTimeSpan.FromRounds(spellEntry.duration)));

      caster.OnCreatureAttack -= CreatureUtils.OnAttackSearingSmite;
      caster.OnCreatureAttack += CreatureUtils.OnAttackSearingSmite;

      FeatUtils.DecrementFeatUses(caster, CustomSkill.SearingSmite);

      return new List<NwGameObject> { caster };
    }
  }
}
