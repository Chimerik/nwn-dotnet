using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeInvocationOcculteSelection()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_INVOCATION_OCCULTE_SELECTION").HasValue)
        {
          if (!windows.TryGetValue("invocationOcculteSelection", out var tech)) windows.Add("invocationOcculteSelection", new InvocationOcculteSelectionWindow(this, oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_INVOCATION_OCCULTE_SELECTION").Value));
          else ((InvocationOcculteSelectionWindow)tech).CreateWindow(oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_INVOCATION_OCCULTE_SELECTION").Value);
        }
      }
    }
  }
}
