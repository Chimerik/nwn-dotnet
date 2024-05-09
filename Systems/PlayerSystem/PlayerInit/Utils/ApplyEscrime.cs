using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyEscrime()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.BotteDefensive))
        {
          oid.LoginCreature.OnCreatureAttack -= BardUtils.OnAttackEscrime;
          oid.LoginCreature.OnCreatureAttack += BardUtils.OnAttackEscrime;
        }
      }
    }
  }
}
