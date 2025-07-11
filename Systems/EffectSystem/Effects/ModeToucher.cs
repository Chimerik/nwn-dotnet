using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ModeToucherEffectTag = "_MODE_TOUCHER_EFFECT";
    public static Effect ModeToucher
    {
      get
      {
        Effect link = Effect.CutsceneGhost();

        link.Tag = ModeToucherEffectTag;
        link.SubType = EffectSubType.Unyielding;

        return link;
      }
    }
  }
}
