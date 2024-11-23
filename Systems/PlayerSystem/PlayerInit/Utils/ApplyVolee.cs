using Anvil.API;
using NWN.Core;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyVolee()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ChasseurVolee))
        {
          oid.LoginCreature.OnCreatureDamage -= RangerUtils.OnDamageVolee;
          oid.LoginCreature.OnCreatureDamage += RangerUtils.OnDamageVolee;
        }
      }
    }
  }
}
