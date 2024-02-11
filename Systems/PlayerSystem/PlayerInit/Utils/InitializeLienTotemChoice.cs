using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeLienTotemChoice()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_LIEN_TOTEM_SELECTION").HasValue)
        {
          if (!windows.TryGetValue("lienTotemSelection", out var value)) windows.Add("lienTotemSelection", new LienTotemSelectionWindow(this));
          else ((LienTotemSelectionWindow)value).CreateWindow();
        }
      }
    }
  }
}
