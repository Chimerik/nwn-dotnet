using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeRangerEnvironmentChoice()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_RANGER_ENVIRONMENT_SELECTION").HasValue)
        {
          if (!windows.TryGetValue("rangerEnvironmentSelection", out var value)) windows.Add("rangerEnvironmentSelection", new RangerEnvironmentSelectionWindow(this));
          else ((RangerEnvironmentSelectionWindow)value).CreateWindow();
        }
      }
    }
  }
}
