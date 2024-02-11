using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeAspectTotemChoice()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_ASPECT_TOTEM_SELECTION").HasValue)
        {
          if (!windows.TryGetValue("aspectTotemSelection", out var value)) windows.Add("aspectTotemSelection", new AspectTotemSelectionWindow(this));
          else ((AspectTotemSelectionWindow)value).CreateWindow();
        }
      }
    }
  }
}
