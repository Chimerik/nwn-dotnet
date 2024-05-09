using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeFightingStyleChoice()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_FIGHTING_STYLE_SELECTION").HasValue)
        {
          if (!windows.TryGetValue("fightingStyleSelection", out var value)) windows.Add("fightingStyleSelection", new FightingStyleSelectionWindow(this, oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_FIGHTING_STYLE_SELECTION").Value));
          else ((FightingStyleSelectionWindow)value).CreateWindow(oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_FIGHTING_STYLE_SELECTION").Value);
        }
      }
    }
  }
}
