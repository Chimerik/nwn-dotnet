using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AssassinateEffectTag = "_ASSASSINATE_EFFECT";
    public static Effect Assassinate
    {
      get
      {
        Effect eff = Effect.Icon(NwGameTables.EffectIconTable.GetRow(163));
        eff.Tag = AssassinateEffectTag;
        return eff;
      }
    }
  }
}
