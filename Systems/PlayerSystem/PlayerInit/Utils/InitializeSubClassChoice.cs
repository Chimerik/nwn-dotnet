using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeSubClassChoice()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SUBCLASS_SELECTION").HasValue)
        {
          if (!windows.TryGetValue("subClassSelection", out var value)) windows.Add("subClassSelection", new SubClassSelectionWindow(this));
          else ((SubClassSelectionWindow)value).CreateWindow();
        }
      }
    }
  }
}
