using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  { 
    private static void BotteTranchanteDeMaitre(NwCreature caster)
    {
      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      caster.GetObjectVariable<LocalVariableInt>(BotteDamageVariable).Value = NwRandom.Roll(Utils.random, 6);
    }
  }
}
