using NWN.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  class DMMode
  {
    public DMMode(NwPlayer oPC)
    {
      if(!oPC.IsPlayerDM)
        PlayerPlugin.ToggleDM(oPC.LoginCreature, 1);
      else
        PlayerPlugin.ToggleDM(oPC.LoginCreature, 0);
    }
  }
}
