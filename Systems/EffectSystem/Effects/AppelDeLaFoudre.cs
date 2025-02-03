using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AppelDeLaFoudreEffectTag = "_APPEL_DE_LA_FOUDRE_EFFECT";
    public static Effect AppelDeLaFoudre
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.AppelDeLaFoudre);
        eff.Tag = AppelDeLaFoudreEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
