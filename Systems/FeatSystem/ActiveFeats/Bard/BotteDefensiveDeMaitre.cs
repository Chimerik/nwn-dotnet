using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void BotteDefensiveDeMaitre(NwCreature caster)
    {
      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      int bonus = NwRandom.Roll(Utils.random, 6);
      caster.GetObjectVariable<LocalVariableInt>(BotteDamageVariable).Value = bonus;
      caster.GetObjectVariable<LocalVariableInt>(BotteDefensiveVariable).Value = bonus;

      caster.OnCreatureAttack -= BardUtils.OnAttackBotteDefensive;
      caster.OnCreatureAttack += BardUtils.OnAttackBotteDefensive;
    }
  }
}
