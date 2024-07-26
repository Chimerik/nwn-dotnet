using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void FrappeDivineVie(NwCreature caster)
    {
      var clerc = caster.GetClassInfo((ClassType)CustomClass.Clerc);

      if (clerc is null || clerc.Level < 1)
        return;

      DamageBonus damage = clerc.Level > 13 ? DamageBonus.Plus2d8 : DamageBonus.Plus1d8;
      caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetFrappeDivineVieEffect(damage));

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Frappe Divine", StringUtils.gold, true, true);
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadHoly));
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.ClercVieFrappeDivine);
    }
  }
}
