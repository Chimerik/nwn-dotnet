using NWN.Enums;
using NWN.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecutePlaceablePersistanceCommand(ChatSystem.ChatEventArgs e, Options.Result options)
    {
      if (NWScript.GetIsDM(e.oSender))
      {
        if (NWNX.Object.GetInt(e.oSender, "_SPAWN_PERSIST") != 0)
        {
          NWNX.Object.DeleteInt(e.oSender, "_SPAWN_PERSIST");
          NWScript.SendMessageToPC(e.oSender, "Persistance des placeables créés par DM désactivée.");
        }
        else
        {
          NWNX.Object.SetInt(e.oSender, "_SPAWN_PERSIST", 1, true);
          NWScript.SendMessageToPC(e.oSender, "Persistance des placeables créés par DM activée.");
        }
      }
    }
}
