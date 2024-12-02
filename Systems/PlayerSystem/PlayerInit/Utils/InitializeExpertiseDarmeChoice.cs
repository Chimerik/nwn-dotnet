using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeExpertiseDarmeChoice()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_EXPERTISE_DARME_SELECTION").HasValue)
        {
          if (!windows.TryGetValue("expertiseDarmeSelection", out var value)) windows.Add("expertiseDarmeSelection", new ExpertiseDarmeSelectionWindow(this, oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_EXPERTISE_DARME_SELECTION").Value));
          else ((ExpertiseDarmeSelectionWindow)value).CreateWindow(oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_EXPERTISE_DARME_SELECTION").Value);
        }
      }
    }
  }
}
