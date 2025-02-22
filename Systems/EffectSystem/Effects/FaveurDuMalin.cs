using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FaveurDuMalinEffectTag = "_FAVEUR_DU_MALIN_EFFECT";
    public static Effect FaveurDuMalin(int faveurId)
    {
      Effect eff = Effect.Icon(CustomEffectIcon.FaveurDuMalin);
      eff.Tag = FaveurDuMalinEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.IntParams[5] = faveurId;
      return eff;
    }
  }
}

