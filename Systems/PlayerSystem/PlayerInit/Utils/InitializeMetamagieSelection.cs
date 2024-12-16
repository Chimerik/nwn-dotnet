using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeMetamagieSelection()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_METAMAGIE_SELECTION").HasValue)
        {
          if (!windows.TryGetValue("metamagieSelection", out var tech)) windows.Add("metamagieSelection", new MetamagieSelectionWindow(this, oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_METAMAGIE_SELECTION").Value));
          else ((MetamagieSelectionWindow)tech).CreateWindow(oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_METAMAGIE_SELECTION").Value);
        }
      }
    }
  }
}
