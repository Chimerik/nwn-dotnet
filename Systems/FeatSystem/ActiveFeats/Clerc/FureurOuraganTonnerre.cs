using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void FureurOuraganTonnerre(NwCreature caster)
    {
      caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.FureurOuraganTonnerre);
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadSonic));
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.ClercFureurOuraganFoudre);
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.ClercFureurOuraganTonnerre);
    }
  }
}
