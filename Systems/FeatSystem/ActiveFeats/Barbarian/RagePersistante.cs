using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void RagePersistante(NwCreature caster)
    {
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpImproveAbilityScore));
      caster.SetFeatRemainingUses((Feat)CustomSkill.BarbarianRagePersistante, 0);
      BarbarianUtils.RestoreBarbarianRage(caster);
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - {StringUtils.ToWhitecolor("Rage Persistante")}", ColorConstants.Orange, true, true);
    }
  }
}
