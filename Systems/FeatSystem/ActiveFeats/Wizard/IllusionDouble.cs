using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void IllusionDouble(NwCreature caster)
    {
      caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.IllusionDouble);
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.IllusionDouble);
    }
  }
}
