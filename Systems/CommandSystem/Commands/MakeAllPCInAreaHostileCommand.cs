using System.Linq;
using NWN.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteMakeAllPCInAreaHostileCommand(ChatSystem.Context ctx, Options.Result options)
    {
      foreach(NwPlayer disliked in NwModule.Instance.Players.Where(p => p.ControlledCreature.Area == ctx.oSender.ControlledCreature.Area && p != ctx.oSender && !ctx.oSender.PartyMembers.Contains(p)))
        NWScript.SetPCDislike(disliked.LoginCreature, ctx.oSender.LoginCreature);
    }
  }
}
