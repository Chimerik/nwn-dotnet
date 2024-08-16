using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeAffiniteElementaireChoice()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_AFFINITE_ELEMENTAIRE_SELECTION").HasValue)
        {
          if (!windows.TryGetValue("ensoDracoAffiniteElementaireSelection", out var value)) windows.Add("ensoDracoAffiniteElementaireSelection", new EnsoDracoAffiniteElementaireSelectionWindow(this));
          else ((EnsoDracoAffiniteElementaireSelectionWindow)value).CreateWindow();
        }
      }
    }
  }
}
