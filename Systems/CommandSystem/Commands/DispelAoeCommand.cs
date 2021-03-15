using System.Linq;
using NWN.API;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteDispelAoeCommand(ChatSystem.Context ctx, Options.Result options)
    {
      foreach (NwAreaOfEffect aoe in NwModule.FindObjectsOfType<NwAreaOfEffect>().Where(aoe => aoe.Creator == ctx.oSender))
        aoe.Destroy();
    }
  }
}
