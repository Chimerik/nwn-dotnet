using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void BenedictionDuFilou(NwCreature caster, NwGameObject oTarget)
    {
      if(oTarget is not NwCreature target)
      {
        caster.LoginPlayer?.SendServerMessage("Cible invalide", ColorConstants.Red);
        return;
      }

      if (caster.DistanceSquared(target) > 81)
      {
        caster.LoginPlayer?.SendServerMessage("Cible hors de portée", ColorConstants.Red);
        return;
      }

      target.ApplyEffect(EffectDuration.Permanent, EffectSystem.BenedictionDuFilou(caster, target));
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Bénédiction du Filou sur {target.Name.ColorString(ColorConstants.Cyan)}", StringUtils.gold, true, true);
    }
  }
}
