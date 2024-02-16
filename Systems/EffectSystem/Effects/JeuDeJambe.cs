using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string JeuDeJambeEffectTag = "_JEU_DE_JAMBE_EFFECT";
    public static readonly Native.API.CExoString jeuDeJambeEffectExoTag = JeuDeJambeEffectTag.ToExoString();
    public static Effect jeuDeJambe
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.ACIncrease);
        eff.Tag = JeuDeJambeEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
