using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ProtectionNaturelleEffectTag = "_PROTECTION_NATURELLE_EFFECT";
    public static Effect ProtectionNaturelle(PlayerSystem.Player druid)
    {
      DamageType damageType = DamageType.Fire;

      if (druid.learnableSkills.ContainsKey(CustomSkill.DruideCercleTerrePolaire))
        damageType = DamageType.Cold;
      else if(druid.learnableSkills.ContainsKey(CustomSkill.DruideCercleTerreTempere))
        damageType = DamageType.Electrical;
      else if (druid.learnableSkills.ContainsKey(CustomSkill.DruideCercleTerreTropicale))
        damageType = CustomDamageType.Poison;

      Effect eff = Effect.LinkEffects(Effect.Immunity(ImmunityType.Poison),
        Effect.DamageImmunityIncrease(damageType, 50));
        
      eff.Tag = ProtectionNaturelleEffectTag;
      eff.SubType = EffectSubType.Unyielding;
      return eff;
    }
  }
}
