namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecutePlaceablePersistanceCommand(ChatSystem.Context chatContext, Options.Result options)
    {
      if (NWScript.GetIsDM(chatContext.oSender))
      {
        if (NWNX.Object.GetInt(chatContext.oSender, "_SPAWN_PERSIST") != 0)
        {
          NWNX.Object.DeleteInt(chatContext.oSender, "_SPAWN_PERSIST");
          NWScript.SendMessageToPC(chatContext.oSender, "Persistance des placeables créés par DM désactivée.");
        }
        else
        {
          NWNX.Object.SetInt(chatContext.oSender, "_SPAWN_PERSIST", 1, true);
          NWScript.SendMessageToPC(chatContext.oSender, "Persistance des placeables créés par DM activée.");
        }
      }
    }
  }
}
