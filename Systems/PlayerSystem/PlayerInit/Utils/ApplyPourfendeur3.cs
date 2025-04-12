using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyPourfendeur3()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.Pourfendeur3))
        {
          oid.LoginCreature.OnCreatureAttack -= RangerUtils.OnAttackPourfendeur3;
          oid.LoginCreature.OnCreatureAttack += RangerUtils.OnAttackPourfendeur3;
        }
      }
    }
  }
}
