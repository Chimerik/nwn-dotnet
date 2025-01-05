using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void ClercMartial(NwCreature caster)
    {
      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.ClercMartialVariable).Value = 1;
      //caster.DecrementRemainingFeatUses((Feat)CustomSkill.ClercMartial);
    }
  }
}
