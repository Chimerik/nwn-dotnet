using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void FrappePrimordiale(NwCreature caster, int featId)
    {
      var druide = caster.GetClassInfo((ClassType)CustomClass.Druid);

      if (druide is null || druide.Level < 1)
        return;

      DamageType damageType;
      switch (featId)
      {
        default: 
          damageType = DamageType.Fire;
          caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadFire));
          break;
        case CustomSkill.DruideFrappePrimordialeFroid: 
          damageType = DamageType.Cold;
          caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadCold)); 
          break;
        case CustomSkill.DruideFrappePrimordialeElec: 
          damageType = DamageType.Electrical;
          caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadElectricity)); 
          break;
        case CustomSkill.DruideFrappePrimordialeTonnerre: 
          damageType = DamageType.Sonic;
          caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadSonic)); 
          break;
      }

      DamageBonus damage = druide.Level > 14 ? DamageBonus.Plus2d8 : DamageBonus.Plus1d8;
      caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.FrappePrimordiale(damage, damageType));

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Frappe Primordiale", StringUtils.gold, true, true);
      DruideUtils.DecrementFrappePrimordiale(caster);
    }
  }
}
