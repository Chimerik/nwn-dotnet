using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeWeaponMasterChoice()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_WEAPON_MASTER_CHOICE_FEAT").HasValue)
        {
          if (!windows.TryGetValue("maitreDarmesSelection", out var value)) windows.Add("maitreDarmesSelection", new MaitreDarmesSelectionWindow(this));
          else ((MaitreDarmesSelectionWindow)value).CreateWindow();
        }
      }
    }
  }
}
