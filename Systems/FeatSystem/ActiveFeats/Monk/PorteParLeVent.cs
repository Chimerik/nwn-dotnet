using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void PorteParLeVent(NwCreature caster)
    {
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Porté par le Vent", StringUtils.gold, true, true);
      FeatUtils.DecrementKi(caster, 4);
    }
  }
}
