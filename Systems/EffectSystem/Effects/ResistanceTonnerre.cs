using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ResistanceTonnerreEffectTag = "_RESISTANCE_TONNERRE_EFFECT";
    public static Effect ResistanceTonnerre
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.TonnerreResistance);
        eff.Tag = ResistanceTonnerreEffectTag;
        return eff;
      }
    }
  }
}
