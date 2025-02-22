using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string BouclierPsychiqueEffectTag = "_BOUCLIER_PSYCHIQUE_EFFECT";
    public static Effect BouclierPsychique
    {
      get
      {
        Effect eff = ResistancePsychique;
        eff.Tag = BouclierPsychiqueEffectTag;
        eff.SubType = EffectSubType.Unyielding;

        return eff;
      }
    }
  }
}

