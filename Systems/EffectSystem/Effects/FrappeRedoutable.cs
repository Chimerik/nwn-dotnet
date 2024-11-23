using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FrappeRedoutableEffectTag = "_FRAPPE_REDOUTABLE_EFFECT";
    public static readonly Native.API.CExoString FrappeRedoutableEffectExoTag = FrappeRedoutableEffectTag.ToExoString();
    public static Effect FrappeRedoutable(byte remainingUse)
    {
      Effect eff = Effect.Icon(EffectIcon.DamageIncrease);
      eff.Tag = FrappeRedoutableEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.IntParams[7] = remainingUse - 1;

      return eff;
    }
  }
}
