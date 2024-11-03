using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void TraverseeInfernale(NwCreature caster)
    {
      caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.TraverseeInfernaleBuff(caster));
    }
  }
}
