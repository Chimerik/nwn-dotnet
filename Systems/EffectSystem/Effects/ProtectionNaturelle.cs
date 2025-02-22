using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ProtectionNaturelleEffectTag = "_PROTECTION_NATURELLE_EFFECT";
    public static Effect ProtectionNaturelle(PlayerSystem.Player druid)
    {
      Effect effRes = ResistanceFeu;

      if (druid.learnableSkills.ContainsKey(CustomSkill.DruideCercleTerrePolaire))
        effRes = ResistanceFroid;
      else if(druid.learnableSkills.ContainsKey(CustomSkill.DruideCercleTerreTempere))
        effRes = ResistanceElec;
      else if (druid.learnableSkills.ContainsKey(CustomSkill.DruideCercleTerreTropicale))
        effRes = ResistancePoison;

      Effect eff = Effect.LinkEffects(Effect.Immunity(ImmunityType.Poison), effRes);        
      eff.Tag = ProtectionNaturelleEffectTag;
      eff.SubType = EffectSubType.Unyielding;
      return eff;
    }
  }
}
