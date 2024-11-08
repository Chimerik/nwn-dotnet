using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Sprint(NwGameObject oCaster, NwSpell spell)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      EffectUtils.RemoveTaggedEffect(oCaster, EffectSystem.SprintEffectTag);

      oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHaste));
      oCaster.ApplyEffect(EffectDuration.Temporary, EffectSystem.sprintEffect, NwTimeSpan.FromRounds(1));

      if (oCaster is NwCreature caster)
      {
        if (caster.KnowsFeat(NwFeat.FromFeatId(CustomSkill.Chargeur)))
          caster.GetObjectVariable<LocalVariableLocation>(EffectSystem.ChargerVariable).Value = caster.Location;

        if (caster.KnowsFeat(NwFeat.FromFeatId(CustomSkill.Mobile)))
          caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.sprintMobileEffect, NwTimeSpan.FromRounds(1));
      }
    }
  }
}
