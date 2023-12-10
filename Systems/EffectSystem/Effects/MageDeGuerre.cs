using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string MageDeGuerreEffectTag = "_MAGE_DE_GUERRE_EFFECT";
    public static Effect MageDeGuerreEffect
    {
      get
      {
        Effect eff = Effect.Icon(NwGameTables.EffectIconTable.GetRow(159));
        eff.Tag = MageDeGuerreEffectTag;
        return eff;
      }
    }
  }
}
