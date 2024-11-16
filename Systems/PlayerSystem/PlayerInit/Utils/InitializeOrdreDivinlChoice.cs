using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeOrdreDivinChoice()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_ORDRE_DIVIN_SELECTION").HasValue)
        {
          if (!windows.TryGetValue("ordreDivinSelection", out var value)) windows.Add("ordreDivinSelection", new OrdreDivinSelectionWindow(this));
          else ((OrdreDivinSelectionWindow)value).CreateWindow();
        }
      }
    }
  }
}
