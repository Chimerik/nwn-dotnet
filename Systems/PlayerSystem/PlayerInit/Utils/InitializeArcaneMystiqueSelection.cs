using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeArcaneMystiqueSelection()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_ARCANE_MYSTIQUE_SELECTION").HasValue)
        {
          if (!windows.TryGetValue("arcaneMystiqueSelection", out var secret10)) windows.Add("arcaneMystiqueSelection", new ArcaneMystiqueSelectionWindow(this, oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_ARCANE_MYSTIQUE_SELECTION").Value));
          else ((ArcaneMystiqueSelectionWindow)secret10).CreateWindow(oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_ARCANE_MYSTIQUE_SELECTION").Value);
        }
      }
    }
  }
}
