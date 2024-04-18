using System.Linq;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeMonkPerfection()
      {
        if (oid.LoginCreature.Classes.Any(c => c.Class.Id == CustomClass.Monk && c.Level > 19))
        {
          oid.OnCombatStatusChange -= MonkUtils.OnCombatMonkRecoverKi;
          oid.OnCombatStatusChange += MonkUtils.OnCombatMonkRecoverKi;
        }
      }
    }
  }
}
