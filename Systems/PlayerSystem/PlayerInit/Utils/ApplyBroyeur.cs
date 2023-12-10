using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyBroyeur()
      {
        if (oid.LoginCreature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.Broyeur)))
        {
          oid.LoginCreature.OnCreatureAttack -= CreatureUtils.OnAttackBroyeur;
          oid.LoginCreature.OnCreatureAttack += CreatureUtils.OnAttackBroyeur;
        }
      }
    }
  }
}
