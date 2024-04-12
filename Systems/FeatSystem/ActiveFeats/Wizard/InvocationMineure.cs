using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void InvocationMineure(NwCreature caster)
    {
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} utilise {"Invocation Mineure".ColorString(ColorConstants.White)}", ColorConstants.Orange, true);
    }
  }
}
