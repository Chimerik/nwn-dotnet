using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string MorsureInfectieuseEffectTag = "_MORSURE_INFECTIEUSE_EFFECT";
    public static readonly Native.API.CExoString MorsureInfectieuseEffectExoTag = MorsureInfectieuseEffectTag.ToExoString();
    public static Effect MorsureInfectieuse
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.FortSaveIncreased);
        eff.Tag = MorsureInfectieuseEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
