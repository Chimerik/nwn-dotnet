using System.Linq;
using NWN.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteMakeAllPCInAreaHostileCommand(ChatSystem.Context ctx, Options.Result options)
    {
      foreach(NwPlayer disliked in ctx.oSender.Area.FindObjectsOfTypeInArea<NwPlayer>().Where(p => p != ctx.oSender && !ctx.oSender.PartyMembers.Contains(p)))
        NWScript.SetPCDislike(disliked, ctx.oSender);
    }
  }
}
