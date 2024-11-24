using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void ViseeStable(NwCreature caster)
    {
      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      EffectSystem.ApplyViseeStable(caster);
    }
  }
}
