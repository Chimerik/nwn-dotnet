using System;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void PacteDuTome(NwCreature caster)
    {
      caster.LoginPlayer?.SendServerMessage("Sorts non implémentés pour le moment");
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Pacte du Tome", StringUtils.brightPurple, true, true);
    }
  }
}
