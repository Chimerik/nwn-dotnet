using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void FureurOuraganFoudre(NwCreature caster)
    {
      caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.FureurOuraganFoudre);
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadElectricity));
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.ClercFureurOuraganFoudre);
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.ClercFureurOuraganTonnerre);
    }
  }
}
