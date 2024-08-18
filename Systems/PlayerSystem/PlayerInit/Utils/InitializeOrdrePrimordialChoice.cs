using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeOrdrePrimordialChoice()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_ORDRE_PRIMORDIAL_SELECTION").HasValue)
        {
          if (!windows.TryGetValue("ordrePrimordialSelection", out var value)) windows.Add("ordrePrimordialSelection", new OrdrePrimordialSelectionWindow(this));
          else ((OrdrePrimordialSelectionWindow)value).CreateWindow();
        }
      }
    }
  }
}
