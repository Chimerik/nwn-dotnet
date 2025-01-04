using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Sprint(NwGameObject oCaster, NwSpell spell)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      EffectUtils.RemoveTaggedEffect(oCaster, EffectSystem.SprintEffectTag);

      oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHaste));

      if (oCaster is NwCreature caster)
      {
        oCaster.ApplyEffect(EffectDuration.Temporary, EffectSystem.Sprint(caster), NwTimeSpan.FromRounds(1));

        if (caster.KnowsFeat(NwFeat.FromFeatId(CustomSkill.Chargeur)))
          caster.GetObjectVariable<LocalVariableLocation>(EffectSystem.ChargerVariable).Value = caster.Location;

        if (caster.KnowsFeat((Feat)CustomSkill.BelluaireEntrainementExceptionnel))
        {
          var companion = caster.GetAssociate(AssociateType.AnimalCompanion);

          if (companion is not null)
          {
            companion.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHaste));
            companion.ApplyEffect(EffectDuration.Temporary, EffectSystem.Sprint(companion), NwTimeSpan.FromRounds(1));
          }
        }
      }
    }
  }
}
