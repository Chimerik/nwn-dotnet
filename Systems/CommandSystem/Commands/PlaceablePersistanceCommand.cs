namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecutePlaceablePersistanceCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (NWScript.GetIsDM(ctx.oSender))
      {
        if (NWNX.Object.GetInt(ctx.oSender, "_SPAWN_PERSIST") != 0)
        {
          NWNX.Object.DeleteInt(ctx.oSender, "_SPAWN_PERSIST");
          NWScript.SendMessageToPC(ctx.oSender, "Persistance des placeables créés par DM désactivée.");
        }
        else
        {
          NWNX.Object.SetInt(ctx.oSender, "_SPAWN_PERSIST", 1, true);
          NWScript.SendMessageToPC(ctx.oSender, "Persistance des placeables créés par DM activée.");
        }
      }
    }
  }
}
