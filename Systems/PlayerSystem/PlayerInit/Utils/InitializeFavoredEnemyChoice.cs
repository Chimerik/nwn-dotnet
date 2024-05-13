using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeFavoredEnemyChoice()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_FAVORED_ENEMY_SELECTION").HasValue)
        {
          if (!windows.TryGetValue("favoredEnemySelection", out var value)) windows.Add("favoredEnemySelection", new FavoredEnemySelectionWindow(this, oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_FAVORED_ENEMY_SELECTION").Value));
          else ((FavoredEnemySelectionWindow)value).CreateWindow(oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_FAVORED_ENEMY_SELECTION").Value);
        }
      }
    }
  }
}
