using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void DivinationSeeEthereal(NwCreature caster)
    {
      caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.DivinationSeeEthereal);
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Troisième oeil - Vision Ethérée", ColorConstants.Purple, true, true);

      caster.SetFeatRemainingUses((Feat)CustomSkill.DivinationSeeInvisibility, 0);
      caster.SetFeatRemainingUses((Feat)CustomSkill.DivinationDarkVision, 0);
    }
  }
}
