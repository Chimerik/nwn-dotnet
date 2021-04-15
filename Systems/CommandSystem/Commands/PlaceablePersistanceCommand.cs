using NWN.API;

namespace NWN.Systems
{
  class PlaceablePersistance
  {
    public PlaceablePersistance(NwPlayer oPC)
    {
      if (oPC.GetLocalVariable<int>("_SPAWN_PERSIST").HasValue)
      {
        oPC.GetLocalVariable<int>("_SPAWN_PERSIST").Delete();
        oPC.SendServerMessage("Persistance des placeables créés par DM désactivée.", Color.BLUE);
      }
      else
      {
        oPC.GetLocalVariable<int>("_SPAWN_PERSIST").Value = 1;
        oPC.SendServerMessage("Persistance des placeables créés par DM activée.", Color.BLUE);
      }
    }
  }
}
