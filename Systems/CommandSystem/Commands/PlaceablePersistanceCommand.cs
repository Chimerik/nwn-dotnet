using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecutePlaceablePersistanceCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (NWScript.GetIsDM(ctx.oSender) == 1)
      {
        if (ObjectPlugin.GetInt(ctx.oSender, "_SPAWN_PERSIST") != 0)
        {
          ObjectPlugin.DeleteInt(ctx.oSender, "_SPAWN_PERSIST");
          NWScript.SendMessageToPC(ctx.oSender, "Persistance des placeables créés par DM désactivée.");
        }
        else
        {
          ObjectPlugin.SetInt(ctx.oSender, "_SPAWN_PERSIST", 1, 1);
          NWScript.SendMessageToPC(ctx.oSender, "Persistance des placeables créés par DM activée.");
        }
      }
    }
  }
}
