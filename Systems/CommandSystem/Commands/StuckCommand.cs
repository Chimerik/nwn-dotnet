using NWN.API;
using NWN.Core;

namespace NWN.Systems
{
  class Unstuck
  {
    public Unstuck(NwPlayer oPC)
    {
      NWScript.JumpToLocation(oPC.Location);
      oPC.SendServerMessage("Tentative de déblocage !", Color.ORANGE);
    }
  }
}
