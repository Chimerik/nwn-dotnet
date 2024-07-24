using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect NoBonusAction
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.RunAction(), Effect.Icon(NwGameTables.EffectIconTable.GetRow(147)));
        eff.Tag = NoBonusActionEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    public const string NoBonusActionEffectTag = "_NO_BONUS_ACTIONS_EFFECT";
  }
}
