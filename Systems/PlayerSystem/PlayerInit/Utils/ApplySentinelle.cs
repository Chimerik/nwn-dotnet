using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplySentinelle()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.Sentinelle))
        {
          oid.LoginCreature.OnCreatureAttack -= CreatureUtils.OnAttackSentinelle;
          oid.LoginCreature.OnCreatureAttack += CreatureUtils.OnAttackSentinelle;
        }
      }
    }
  }
}
