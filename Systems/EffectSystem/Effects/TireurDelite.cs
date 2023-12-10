using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string TireurDeliteEffectTag = "_TIREUR_DELITE_EFFECT";
    public static readonly Native.API.CExoString TireurDeliteExoTag = "_TIREUR_DELITE_EFFECT".ToExoString();
    public static Effect TireurDeliteEffect
    {
      get
      {
        Effect eff = Effect.Icon(NwGameTables.EffectIconTable.GetRow(158));
        eff.Tag = TireurDeliteEffectTag;
        return eff;
      }
    }
  }
}
