using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeBelluaireCompanionChoice()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_BELLUAIRE_COMPANION_SELECTION").HasValue)
        {
          if (!windows.TryGetValue("belluaireCompanionSelection", out var value)) windows.Add("belluaireCompanionSelection", new BelluaireCompanionSelectionWindow(this));
          else ((BelluaireCompanionSelectionWindow)value).CreateWindow();
        }
      }
    }
  }
}
