using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ChillEffectTag = "_CHILL_EFFECT";
    public static Effect Chill
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.Chilled), VulnerabiliteFroid, ResistanceFeu);
        eff.Tag = ChillEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
