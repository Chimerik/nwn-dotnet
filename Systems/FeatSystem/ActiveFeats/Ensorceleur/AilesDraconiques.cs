using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void AilesDraconiques(NwCreature caster)
    {
      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      if (caster.WingType == CreatureWingType.None)
      {
        if (caster.KnowsFeat((Feat)CustomSkill.EnsoDracoAffiniteAcide))
        {
          caster.WingType = (CreatureWingType)65;
          caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadAcid));
        }
        else if (caster.KnowsFeat((Feat)CustomSkill.EnsoDracoAffiniteFroid))
        {
          caster.WingType = (CreatureWingType)64;
          caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPulseCold));
        }
        else if (caster.KnowsFeat((Feat)CustomSkill.EnsoDracoAffiniteFeu))
        {
          caster.WingType = (CreatureWingType)68;
          caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPulseFire));
        }
        else if (caster.KnowsFeat((Feat)CustomSkill.EnsoDracoAffiniteElec))
        {
          caster.WingType = (CreatureWingType)67;
          caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadElectricity));
        }
        else if (caster.KnowsFeat((Feat)CustomSkill.EnsoDracoAffinitePoison))
        {
          caster.WingType = (CreatureWingType)66;
          caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPoisonS));
        }
      }
      else
      {
        caster.WingType = CreatureWingType.None;
        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpReduceAbilityScore));
      }
    }
  }
}
