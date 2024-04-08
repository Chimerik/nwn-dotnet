using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void DivinationSeeInvisible(NwCreature caster)
    {
      caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.DivinationSeeInvisibility);
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Troisième oeil - Voir l'invisible", ColorConstants.Purple, true, true);

      caster.SetFeatRemainingUses((Feat)CustomSkill.DivinationDarkVision, 0);
      caster.SetFeatRemainingUses((Feat)CustomSkill.DivinationSeeEthereal, 0);
    }
  }
}
