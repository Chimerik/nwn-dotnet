using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeHunterDefenseSuperieureChoice()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_HUNTER_DEFENSE_SUPERIEURE_SELECTION").HasValue)
        {
          if (!windows.TryGetValue("hunterDefenseSuperieureSelection", out var value)) windows.Add("hunterDefenseSuperieureSelection", new HunterDefenseSuperieureSelectionWindow(this));
          else ((HunterDefenseSuperieureSelectionWindow)value).CreateWindow();
        }
      }
    }
  }
}
