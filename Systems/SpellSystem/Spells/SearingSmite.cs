using System.Collections.Generic;
using Anvil.API;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void SearingSmite(SpellEvents.OnSpellCast onSpellCast, SpellEntry spellEntry)
    {
      if (onSpellCast.Caster is not NwCreature caster)
        return;

      StringUtils.ForceBroadcastSpellCasting(caster, onSpellCast.Spell);
      caster.Location.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.VisualEffect(VfxType.ImpPulseFire), Effect.VisualEffect(VfxType.ImpFlameM)));

      NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.searingSmiteAttack, NwTimeSpan.FromRounds(spellEntry.duration)));
      EffectSystem.ApplyConcentrationEffect(caster, onSpellCast.Spell.Id, new List<NwGameObject> { caster }, spellEntry.duration);

      caster.OnCreatureAttack -= CreatureUtils.OnAttackSearingSmite;
      caster.OnCreatureAttack += CreatureUtils.OnAttackSearingSmite;

      FeatUtils.DecrementFeatUses(caster, CustomSkill.SearingSmite);
      CreatureUtils.HandleBonusActionCooldown(caster);
    }
  }
}
