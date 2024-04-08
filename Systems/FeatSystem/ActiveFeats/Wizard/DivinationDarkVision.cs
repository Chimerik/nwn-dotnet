using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void DivinationDarkVision(NwCreature caster)
    {
      caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.DivinationDarkVision);
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Troisième oeil - Vision dans le noir", ColorConstants.Purple, true, true);

      caster.SetFeatRemainingUses((Feat)CustomSkill.DivinationSeeInvisibility, 0);
      caster.SetFeatRemainingUses((Feat)CustomSkill.DivinationSeeEthereal, 0);
    }
  }
}
