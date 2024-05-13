using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyMonkOpportunist()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.MonkOpportuniste))
        {
          oid.LoginCreature.OnCreatureAttack -= MonkUtils.OnAttackMonkOpportunist;
          oid.LoginCreature.OnCreatureAttack += MonkUtils.OnAttackMonkOpportunist;
        }
      }
    }
  }
}
