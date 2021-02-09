using NWN.API;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecutePlaceablePersistanceCommand(ChatSystem.Context ctx, Options.Result options)
    {
       NwPlayer oPC = ctx.oSender.ToNwObject<NwPlayer>();
       if (oPC.GetLocalVariable<int>("_SPAWN_PERSIST").HasValue)
       {
            oPC.GetLocalVariable<int>("_SPAWN_PERSIST").Delete();
            oPC.SendServerMessage("Persistance des placeables créés par DM désactivée.");
        }
        else
        {
            oPC.GetLocalVariable<int>("_SPAWN_PERSIST").Value = 1;
            oPC.SendServerMessage("Persistance des placeables créés par DM activée.");
        }
    }
  }
}
