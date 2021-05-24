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
        PlayerPlugin.SetAlwaysWalk(oPC.LoginCreature, 1);
        ObjectPlugin.SetInt(oPC.LoginCreature, "_ALWAYS_WALK", 1, 1);
        NWScript.SendMessageToPC(oPC.LoginCreature, "Vous avez activé le mode marche.");
      }
      else
      {
        PlayerPlugin.SetAlwaysWalk(oPC.LoginCreature, 0);
        ObjectPlugin.DeleteInt(oPC.LoginCreature, "_ALWAYS_WALK");
        NWScript.SendMessageToPC(oPC.LoginCreature, "Vous avez désactivé le mode marche.");
      }
    }
  }
}
