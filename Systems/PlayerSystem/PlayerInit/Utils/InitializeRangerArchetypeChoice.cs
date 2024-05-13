using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeRangerArchetypeChoice()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_RANGER_ARCHETYPE_SELECTION").HasValue)
        {
          if (!windows.TryGetValue("rangerArchetypeSelection", out var value)) windows.Add("rangerArchetypeSelection", new RangerArchetypeSelectionWindow(this));
          else ((RangerArchetypeSelectionWindow)value).CreateWindow();
        }
      }
    }
  }
}
