using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AgiliteHalfelinEffectTag = "_AGILITE_HALFELIN_EFFECT";
    public static Effect AgiliteHalfelin
    {
      get
      {
        Effect eff = Effect.CutsceneGhost();
        eff.Tag = AgiliteHalfelinEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}

