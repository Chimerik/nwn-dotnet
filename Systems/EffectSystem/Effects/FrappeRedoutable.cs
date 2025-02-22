using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FrappeRedoutableEffectTag = "_FRAPPE_REDOUTABLE_EFFECT";

    public static Effect FrappeRedoutable(byte remainingUse)
    {
      Effect eff = Effect.Icon(CustomEffectIcon.FrappeRedoutable);
      eff.Tag = FrappeRedoutableEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.CasterLevel = remainingUse - 1;

      return eff;
    }
  }
}
