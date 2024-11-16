using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeFrappesBeniesChoice()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_FRAPPES_BENIES_SELECTION").HasValue)
        {
          if (!windows.TryGetValue("frappesBeniesSelection", out var value)) windows.Add("frappesBeniesSelection", new FrappesBeniesSelectionWindow(this));
          else ((FrappesBeniesSelectionWindow)value).CreateWindow();
        }
      }
    }
  }
}
