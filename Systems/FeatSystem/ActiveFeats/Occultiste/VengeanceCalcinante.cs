using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void VengeanceCalcinante(NwCreature caster)
    {
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.VengeanceCalcinante);
      caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.VengeanceCalcinanteAura(caster));
      UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>().SetRadius(18);
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Vengeance Calcinante", StringUtils.gold, true, true);
    }
  }
}
