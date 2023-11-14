using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeAbilityImprovementFeat()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_ABILITY_IMPROVEMENT_FEAT").HasValue)
        {
          if (!windows.ContainsKey("abilityImprovement")) windows.Add("abilityImprovement", new AbilityImprovementWindow(this));
          else ((AbilityImprovementWindow)windows["abilityImprovement"]).CreateWindow();
        }
      }
    }
  }
}
