using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void DivinationDarkVision(NwCreature caster)
    {
      NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.DivinationDarkVision));
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Troisième oeil - Vision dans le noir", ColorConstants.Magenta, true, true);

      caster.DecrementRemainingFeatUses((Feat)CustomSkill.DivinationSeeInvisibility);
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.DivinationSeeEthereal);
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.DivinationDarkVision);
    }
  }
}
