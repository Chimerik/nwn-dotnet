using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyTueurDeMage()
      {
        if (oid.LoginCreature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.TueurDeMage)))
        {
          oid.LoginCreature.OnCreatureAttack -= CreatureUtils.OnAttackTueurDeMage;
          oid.LoginCreature.OnCreatureAttack += CreatureUtils.OnAttackTueurDeMage;
        }
      }
    }
  }
}
