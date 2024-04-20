using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyFrappeMeurtriere()
      {
        if (oid.LoginCreature.Classes.Any(c => c.Class.ClassType == ClassType.Rogue && c.Level > 16))
        {
          oid.LoginCreature.OnCreatureDamage -= CreatureUtils.OnDamageFrappeMeurtriere;
          oid.LoginCreature.OnCreatureDamage += CreatureUtils.OnDamageFrappeMeurtriere;
        }
      }
    }
  }
}
