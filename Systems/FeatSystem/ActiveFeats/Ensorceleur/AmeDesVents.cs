using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void AmeDesVents(NwCreature caster)
    {
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Âme des Vents", StringUtils.gold, true, true);
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.EnsoAmeDesVents);
    }
  }
}
