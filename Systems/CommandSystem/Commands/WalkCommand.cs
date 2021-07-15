using Anvil.API;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  class WalkMode
  {
    public WalkMode(NwPlayer oPC)
    {
      if (oPC.LoginCreature.GetObjectVariable<PersistentVariableInt>("_ALWAYS_WALK").HasNothing)
      {
        PlayerPlugin.SetAlwaysWalk(oPC.ControlledCreature, 1);
        oPC.LoginCreature.GetObjectVariable<PersistentVariableInt>("_ALWAYS_WALK").Value = 1;
        oPC.SendServerMessage("Mode marche activé.", ColorConstants.Pink);
      }
      else
      {
        PlayerPlugin.SetAlwaysWalk(oPC.LoginCreature, 0);
        oPC.LoginCreature.GetObjectVariable<PersistentVariableInt>("_ALWAYS_WALK").Delete();
        oPC.SendServerMessage("Mode marche désactivé.", ColorConstants.Pink);
      }
    }
  }
}
