using System.Linq;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeMonkPercetion()
      {
        if (oid.LoginCreature.Classes.Any(c => c.Class.ClassType == Anvil.API.ClassType.Monk && c.Level > 19))
        {
          oid.OnCombatStatusChange -= MonkUtils.OnCombatMonkRecoverKi;
          oid.OnCombatStatusChange += MonkUtils.OnCombatMonkRecoverKi;
        }
      }
    }
  }
}
