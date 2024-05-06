using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeMagicalSecretSelection()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_MAGICAL_SECRET_SELECTION").HasValue)
        {
          if (!windows.TryGetValue("bardMagicalSecretSelection", out var secret10)) windows.Add("bardMagicalSecretSelection", new BardMagicalSecretSelectionWindow(this, oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_MAGICAL_SECRET_SELECTION").Value));
          else ((BardMagicalSecretSelectionWindow)secret10).CreateWindow(oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_MAGICAL_SECRET_SELECTION").Value);
        }
      }
    }
  }
}
