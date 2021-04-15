using NWN.API;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  class WalkMode
  {
    public WalkMode(NwPlayer oPC)
    {
      if (ObjectPlugin.GetInt(oPC, "_ALWAYS_WALK") == 0)
      {
        PlayerPlugin.SetAlwaysWalk(oPC, 1);
        ObjectPlugin.SetInt(oPC, "_ALWAYS_WALK", 1, 1);
        NWScript.SendMessageToPC(oPC, "Vous avez activé le mode marche.");
      }
      else
      {
        PlayerPlugin.SetAlwaysWalk(oPC, 0);
        ObjectPlugin.DeleteInt(oPC, "_ALWAYS_WALK");
        NWScript.SendMessageToPC(oPC, "Vous avez désactivé le mode marche.");
      }
    }
  }
}
