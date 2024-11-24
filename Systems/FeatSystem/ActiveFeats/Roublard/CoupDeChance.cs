using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void CoupDeChance(NwCreature caster)
    {
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.RoublardCoupDeChance);
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Coup de Chance !", StringUtils.gold, true, true);
    }
  }
}
