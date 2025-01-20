using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void LienElementaire(NwCreature caster)
    {
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Lien Elémentaire", StringUtils.gold, true, true);
    }
  }
}
