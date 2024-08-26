using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeFureurElementaireChoice()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_FUREUR_ELEMENTAIRE_SELECTION").HasValue)
        {
          if (!windows.TryGetValue("fureurElementaireSelection", out var value)) windows.Add("fureurElementaireSelection", new FureurElementaireSelectionWindow(this));
          else ((FureurElementaireSelectionWindow)value).CreateWindow();
        }
      }
    }
  }
}
