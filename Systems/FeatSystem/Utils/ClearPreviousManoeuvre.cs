using Anvil.API;

namespace NWN
{
  public static partial class FeatUtils
  {
    public static void ClearPreviousManoeuvre(NwCreature creature)
    {
      creature.OnCreatureAttack -= CreatureUtils.OnAttackDesarmement;
      creature.OnCreatureAttack -= CreatureUtils.OnAttackMenacante;
      creature.OnCreatureAttack -= CreatureUtils.OnAttackProvocation;
      creature.OnCreatureAttack -= CreatureUtils.OnAttackRenversement;
      creature.GetObjectVariable<LocalVariableInt>(CreatureUtils.ManoeuvreBalayageTargetVariable).Delete();
      creature.GetObjectVariable<LocalVariableInt>(CreatureUtils.ManoeuvreTypeVariable).Delete();
      creature.GetObjectVariable<LocalVariableInt>(CreatureUtils.ManoeuvreDiceVariable).Delete();
    }
  }
}
