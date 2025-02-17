using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ConstitutionInfernaleEffectTag = "_CONSTITUTION_INFERNALE_EFFECT";
    public static Effect ConstitutionInfernale
    {
      get
      {
        Effect eff = Effect.LinkEffects(ResistancePoison, ResistanceFroid);
        eff.Tag = ConstitutionInfernaleEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
