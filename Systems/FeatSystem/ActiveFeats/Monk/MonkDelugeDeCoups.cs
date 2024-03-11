using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void MonkDelugeDeCoups(NwCreature caster)
    {
      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.MonkDelugeVariable).Value = 1;
    }
  }
}
