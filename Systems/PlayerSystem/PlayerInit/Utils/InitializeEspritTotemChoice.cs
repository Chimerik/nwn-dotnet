using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeEspritTotemChoice()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_ESPRIT_TOTEM_SELECTION").HasValue)
        {
          if (!windows.TryGetValue("espritTotemSelection", out var value)) windows.Add("espritTotemSelection", new EspritTotemSelectionWindow(this));
          else ((EspritTotemSelectionWindow)value).CreateWindow();
        }
      }
    }
  }
}
