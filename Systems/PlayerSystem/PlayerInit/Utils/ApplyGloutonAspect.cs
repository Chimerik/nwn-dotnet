using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyGloutonAspect()
      {
        oid.LoginCreature.OnCreatureAttack -= CreatureUtils.OnAttackAspectGlouton;

        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.TotemAspectGlouton))
          oid.LoginCreature.OnCreatureAttack += CreatureUtils.OnAttackAspectGlouton;
      }
    }
  }
}
