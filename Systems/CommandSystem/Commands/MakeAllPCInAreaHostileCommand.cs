using System.Linq;
using NWN.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteMakeAllPCInAreaHostileCommand(ChatSystem.Context ctx, Options.Result options)
    {
      NwPlayer oPC = ctx.oSender.ToNwObject<NwPlayer>();

      foreach(NwPlayer disliked in oPC.Area.FindObjectsOfTypeInArea<NwPlayer>().Where(p => p != oPC && !oPC.PartyMembers.Contains(p)))
      {
        NWScript.SetPCDislike(disliked, oPC);
      }
    }
  }
}
