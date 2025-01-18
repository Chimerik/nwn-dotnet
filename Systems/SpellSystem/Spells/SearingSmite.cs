using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void SearingSmite(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwFeat feat)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      EffectUtils.ClearChatimentEffects(caster);
      caster.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.VisualEffect(VfxType.ImpPulseFire), Effect.VisualEffect(VfxType.ImpFlameM)));

      NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.searingSmiteAttack));

      caster.OnCreatureAttack -= CreatureUtils.OnAttackSearingSmite;
      caster.OnCreatureAttack += CreatureUtils.OnAttackSearingSmite;

      if(feat is not null)
        FeatUtils.DecrementFeatUses(caster, feat.Id);
    }
  }
}
