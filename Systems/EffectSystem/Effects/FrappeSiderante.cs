using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FrappeSideranteEffectTag = "_FRAPPE_SIDERANTE_EFFECT";
    public static Effect FrappeSiderante
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.SavingThrowDecrease);
        eff.Tag = FrappeSideranteEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
