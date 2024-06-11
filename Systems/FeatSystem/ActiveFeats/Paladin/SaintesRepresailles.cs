using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void SaintesRepresailles(NwCreature caster, NwGameObject oTarget)
    {
      if(oTarget is not NwCreature target)
      {
        caster.LoginPlayer?.SendServerMessage("Cible invalide", ColorConstants.Red);
        return;
      }

      target.ApplyEffect(EffectDuration.Temporary, Effect.DamageShield(0, DamageBonus.Plus1d4, DamageType.Divine), NwTimeSpan.FromRounds(2));
      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSuperHeroism));

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} Saintes Représailles sur {target.Name.ColorString(ColorConstants.Cyan)}", StringUtils.gold, true, true);

      PaladinUtils.ConsumeOathCharge(caster);
    }
  }
}
