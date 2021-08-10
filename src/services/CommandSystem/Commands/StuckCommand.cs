using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  class Unstuck
  {
    public Unstuck(NwPlayer oPC)
    {
      oPC.ControlledCreature.Location = oPC.ControlledCreature.Location;
      oPC.SendServerMessage("Tentative de déblocage !", ColorConstants.Orange);
    }
  }
}
