using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeMaitriseDesSortsChoice()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_MAITRISE_DES_SORTS_SELECTION").HasValue)
        {
          if (!windows.TryGetValue("maitriseDesSorts", out var masterSpell)) windows.Add("maitriseDesSorts", new MaitriseDesSortsWindow(this));
          else ((MaitriseDesSortsWindow)masterSpell).CreateWindow();
        }
      }
    }
  }
}
