using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyElectrocution()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ClercElectrocution))
        {
          oid.LoginCreature.OnCreatureDamage -= ClercUtils.OnDamageElectrocution;
          oid.LoginCreature.OnCreatureDamage += ClercUtils.OnDamageElectrocution;
        }
      }
    }
  }
}
