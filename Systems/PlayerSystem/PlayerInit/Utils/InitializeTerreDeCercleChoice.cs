using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeTerreDeCercleChoice()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_TERRE_DE_CERCLE_SELECTION").HasValue)
        {
          if (!windows.TryGetValue("terrainDeCercleSelection", out var value)) windows.Add("terrainDeCercleSelection", new TerrainDeCercleSelectionWindow(this));
          else ((TerrainDeCercleSelectionWindow)value).CreateWindow();
        }
      }
    }
  }
}
