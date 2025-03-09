using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeHighElfCantripSelection()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_HIGHELF_CANTRIP_SELECTION").HasValue)
        {
          if (!windows.TryGetValue("highElfCantripSelection", out var cantrip)) windows.Add("highElfCantripSelection", new HighElfCantripSelectionWindow(this));
          else ((HighElfCantripSelectionWindow)cantrip).CreateWindow();
        }
      }
    }
  }
}
