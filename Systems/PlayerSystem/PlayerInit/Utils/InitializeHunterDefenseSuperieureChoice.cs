using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeHunterDefenseSuperieureChoice()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ChasseurDefenseSuperieure))
        {
          oid.LoginCreature.OnDamaged -= RangerUtils.OnDamageDefenseSuperieure;
          oid.LoginCreature.OnDamaged += RangerUtils.OnDamageDefenseSuperieure;
        }
      }
    }
  }
}
