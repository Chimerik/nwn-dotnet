using NWN.API;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecutePlaceablePersistanceCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (!ctx.oSender.IsDM && ctx.oSender.IsDMPossessed && ctx.oSender.IsPlayerDM)
        return;

      if (ctx.oSender.GetLocalVariable<int>("_SPAWN_PERSIST").HasValue)
      {
        ctx.oSender.GetLocalVariable<int>("_SPAWN_PERSIST").Delete();
        ctx.oSender.SendServerMessage("Persistance des placeables créés par DM désactivée.", Color.BLUE);
      }
      else
      {
        ctx.oSender.GetLocalVariable<int>("_SPAWN_PERSIST").Value = 1;
        ctx.oSender.SendServerMessage("Persistance des placeables créés par DM activée.", Color.BLUE);
      }
    }
  }
}
