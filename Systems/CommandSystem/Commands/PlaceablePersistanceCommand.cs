using NWN.API;

namespace NWN.Systems
{
  class PlaceablePersistance
  {
    public PlaceablePersistance(NwPlayer oPC)
    {
      if (oPC.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPAWN_PERSIST").HasValue)
      {
        oPC.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPAWN_PERSIST").Delete();
        oPC.SendServerMessage("Persistance des placeables créés par DM désactivée.", ColorConstants.Blue);
      }
      else
      {
        oPC.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPAWN_PERSIST").Value = 1;
        oPC.SendServerMessage("Persistance des placeables créés par DM activée.", ColorConstants.Blue);
      }
    }
  }
}
