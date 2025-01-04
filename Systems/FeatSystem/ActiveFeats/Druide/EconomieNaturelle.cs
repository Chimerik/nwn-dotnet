using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void EconomieNaturelle(NwCreature caster)
    {
      NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.EconomieNaturelle));
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Economie Naturelle", StringUtils.gold, true, true);

      caster.DecrementRemainingFeatUses((Feat)CustomSkill.DruideEconomieNaturelle);
    }
  }
}
