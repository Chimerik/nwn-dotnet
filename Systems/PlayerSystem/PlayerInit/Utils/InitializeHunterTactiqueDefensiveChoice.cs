using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeHunterTactiqueDefensiveChoice()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_HUNTER_TACTIQUE_DEFENSIVE_SELECTION").HasValue)
        {
          if (!windows.TryGetValue("hunterTactiqueDefensiveSelection", out var value)) windows.Add("hunterTactiqueDefensiveSelection", new HunterTactiqueDefensiveSelectionWindow(this));
          else ((HunterTactiqueDefensiveSelectionWindow)value).CreateWindow();
        }
      }
    }
  }
}
