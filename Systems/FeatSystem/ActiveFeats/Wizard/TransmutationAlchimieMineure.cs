using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void TransmutationAlchimieMineure(NwCreature caster)
    {
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} utilise {"Alchimie Mineure".ColorString(ColorConstants.White)}", ColorConstants.Orange, true);
    }
  }
}
