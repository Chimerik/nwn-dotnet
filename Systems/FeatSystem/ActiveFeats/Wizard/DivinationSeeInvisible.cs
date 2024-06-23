using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void DivinationSeeInvisible(NwCreature caster)
    {
      NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.DivinationSeeInvisibility));
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Troisième oeil - Voir l'invisible", ColorConstants.Magenta, true, true);

      caster.DecrementRemainingFeatUses((Feat)CustomSkill.DivinationDarkVision);
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.DivinationSeeEthereal);
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.DivinationSeeInvisibility);
    }
  }
}
