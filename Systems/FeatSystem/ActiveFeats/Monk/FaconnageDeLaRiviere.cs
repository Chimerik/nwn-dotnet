using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void FaconnageDeLaRiviere(NwCreature caster)
    {
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Façonnage de la Rivière", StringUtils.gold, true, true);
      FeatUtils.DecrementKi(caster);
    }
  }
}
