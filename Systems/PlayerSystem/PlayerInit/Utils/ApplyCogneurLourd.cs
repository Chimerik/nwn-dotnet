using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyCogneurLourd()
      {
        if (oid.LoginCreature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.CogneurLourd)))
        {
          oid.LoginCreature.OnCreatureAttack -= CreatureUtils.OnAttackCogneurLourd;
          oid.LoginCreature.OnCreatureAttack += CreatureUtils.OnAttackCogneurLourd;
        }
      }
    }
  }
}
