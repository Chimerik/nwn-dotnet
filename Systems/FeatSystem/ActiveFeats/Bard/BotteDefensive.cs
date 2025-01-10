using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void BotteDefensive(NwCreature caster, int featId)
    {
      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.BotteSecrete(caster, featId == CustomSkill.BotteDefensiveDeMaitre));
    }
  }
}
