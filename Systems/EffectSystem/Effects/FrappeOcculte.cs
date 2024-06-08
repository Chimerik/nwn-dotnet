using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FrappeOcculteEffectTag = "_FRAPPE_OCCULTE_EFFECT";
    public static Effect FrappeOcculte
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.SavingThrowDecrease);
        eff.Tag = FrappeOcculteEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
