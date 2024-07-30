using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeTechniqueElementaireSelection()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_TECHNIQUE_ELEMENTAIRE_SELECTION").HasValue)
        {
          if (!windows.TryGetValue("techniqueElementaireSelection", out var tech)) windows.Add("techniqueElementaireSelection", new TechniqueElementaireSelectionWindow(this, oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_TECHNIQUE_ELEMENTAIRE_SELECTION").Value));
          else ((TechniqueElementaireSelectionWindow)tech).CreateWindow(oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_TECHNIQUE_ELEMENTAIRE_SELECTION").Value);
        }
      }
    }
  }
}
