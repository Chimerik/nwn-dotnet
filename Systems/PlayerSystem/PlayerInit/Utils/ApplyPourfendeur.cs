using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyPourfendeur()
      {
        if (oid.LoginCreature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.Pourfendeur)))
        {
          oid.LoginCreature.OnCreatureAttack -= CreatureUtils.OnAttackPourfendeur;
          oid.LoginCreature.OnCreatureAttack += CreatureUtils.OnAttackPourfendeur;

          oid.LoginCreature.OnCreatureDamage -= CreatureUtils.OnDamagePourfendeur;
          oid.LoginCreature.OnCreatureDamage += CreatureUtils.OnDamagePourfendeur;
        }
      }
    }
  }
}
