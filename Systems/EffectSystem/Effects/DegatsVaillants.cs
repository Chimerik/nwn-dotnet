using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string DegatsVaillanteEffectTag = "_DEGATS_VAILLANTE_EFFECT";
    public static readonly Native.API.CExoString degatsVaillanteEffectExoTag = DegatsVaillanteEffectTag.ToExoString();
    public static Effect GetDegatsVaillanteEffect(int bonus)
    {
      Effect eff = Effect.Icon((EffectIcon)171);
      eff.Tag = DegatsVaillanteEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.CasterLevel = bonus;
      return eff;
    }
  }
}
