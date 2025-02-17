using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ImageMiroirEffectTag = "_IMAGE_MIROIR_EFFECT";
    public static readonly Native.API.CExoString ImageMiroirEffectExoTag = ImageMiroirEffectTag.ToExoString();
    public static Effect ImageMiroir
    {
      get
      {
        Effect eff = Effect.VisualEffect(VfxType.DurGhostlyVisage);
        eff.Tag = ImageMiroirEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.ImageMiroir);
        return eff;
      }
    }
  }
}
