using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeRangerKnacksSelection()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_RANGER_KNACKS_SELECTION").HasValue)
        {
          if (!windows.TryGetValue("rangerKnacksSelection", out var tech)) windows.Add("rangerKnacksSelection", new RangerKnacksSelectionWindow(this, oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_RANGER_KNACKS_SELECTION").Value));
          else ((RangerKnacksSelectionWindow)tech).CreateWindow(oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_RANGER_KNACKS_SELECTION").Value);
        }
      }
    }
  }
}
