using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AmeRadieuseEffectTag = "_AME_RADIEUSE_EFFECT";
    public static Effect AmeRadieuse
    {
      get
      {
        Effect eff = ResistanceRadiant;
        eff.Tag = AmeRadieuseEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
