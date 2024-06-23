using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void DivinationSeeEthereal(NwCreature caster)
    {
      NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.DivinationSeeEthereal));
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Troisième oeil - Vision Ethérée", ColorConstants.Magenta, true, true);

      caster.DecrementRemainingFeatUses((Feat)CustomSkill.DivinationSeeInvisibility);
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.DivinationDarkVision);
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.DivinationSeeEthereal);
    }
  }
}
