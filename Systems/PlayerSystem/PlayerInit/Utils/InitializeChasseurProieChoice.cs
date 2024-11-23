using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeChasseurProieChoice()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_HUNTER_PROIE_SELECTION").HasValue)
        {
          if (!windows.TryGetValue("hunterProieSelection", out var value)) windows.Add("hunterProieSelection", new HunterProieSelectionWindow(this));
          else ((HunterProieSelectionWindow)value).CreateWindow();
        }
      }
    }
  }
}
