using System;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void VisionDuPasse(NwCreature caster)
    {
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Vision du Passé", StringUtils.gold, true, true);
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.ClercVisionDuPasse);
    }
  }
}
