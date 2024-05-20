using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyDefenseAdaptative()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ChasseurDefenseAdaptative))
        {
          oid.LoginCreature.OnDamaged -= RangerUtils.OnDamageDefenseAdaptative;
          oid.LoginCreature.OnDamaged += RangerUtils.OnDamageDefenseAdaptative;
        }
      }
    }
  }
}
