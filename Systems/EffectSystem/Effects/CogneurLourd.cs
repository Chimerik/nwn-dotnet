using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string CogneurLourdEffectTag = "_COGNEUR_LOURD_EFFECT";
    public static readonly Native.API.CExoString CogneurLourdExoTag = "_COGNEUR_LOURD_EFFECT".ToExoString();
    public static Effect cogneurLourdEffect
    {
      get
      {
        Effect eff = Effect.Icon(NwGameTables.EffectIconTable.GetRow(156));
        eff.Tag = CogneurLourdEffectTag;
        return eff;
      }
    }
  }
}
