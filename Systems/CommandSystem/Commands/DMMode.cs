using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  class DMMode
  {
    public DMMode(NwPlayer oPC)
    {
      if (!oPC.IsPlayerDM)
        oPC.IsPlayerDM = true;
      else
        oPC.IsPlayerDM = false;
    }
  }
}
