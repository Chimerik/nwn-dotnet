using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyGloutonAspect()
      {
        oid.LoginCreature.OnCreatureAttack -= CreatureUtils.OnAttackAspectGlouton;

        if (oid.LoginCreature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.TotemAspectGlouton)))
          oid.LoginCreature.OnCreatureAttack += CreatureUtils.OnAttackAspectGlouton;
      }
    }
  }
}
