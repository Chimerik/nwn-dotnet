using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void GuideTempete(NwCreature caster)
    {
      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Guide Tempête", StringUtils.gold, true, true);
    }
  }
}
