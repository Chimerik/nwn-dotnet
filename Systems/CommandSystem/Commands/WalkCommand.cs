using NWN.API;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  class WalkMode
  {
    public WalkMode(NwPlayer oPC)
    {
      if (ObjectPlugin.GetInt(oPC.LoginCreature, "_ALWAYS_WALK") == 0)
      {
        PlayerPlugin.SetAlwaysWalk(oPC.ControlledCreature, 1);
        ObjectPlugin.SetInt(oPC.LoginCreature, "_ALWAYS_WALK", 1, 1);
        oPC.SendServerMessage("Mode marche activé.", ColorConstants.Pink);
      }
      else
      {
        PlayerPlugin.SetAlwaysWalk(oPC.LoginCreature, 0);
        ObjectPlugin.DeleteInt(oPC.LoginCreature, "_ALWAYS_WALK");
        oPC.SendServerMessage("Mode marche désactivé.", ColorConstants.Pink);
      }
    }
  }
}
