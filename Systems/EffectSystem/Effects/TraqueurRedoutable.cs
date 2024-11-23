using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string TraqueurRedoutableEffectTag = "_TRAQUEUR_REDOUTABLE_EFFECT";
    public static Effect TraqueurRedoutable
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.MovementSpeedIncrease(15), Effect.Icon(EffectIcon.MovementSpeedIncrease));
        eff.Tag = TraqueurRedoutableEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
