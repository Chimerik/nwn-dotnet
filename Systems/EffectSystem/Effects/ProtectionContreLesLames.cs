
using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ProtectionContreLesLamesEffectTag = "_PROTECTION_CONTRE_LES_LAMES_EFFECT";
    public static readonly Native.API.CExoString ProtectionContreLesLamesEffectExoTag = ProtectionContreLesLamesEffectTag.ToExoString();
    public static Effect ProtectionContreLesLames
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.ACIncrease);
        eff.Tag = ProtectionContreLesLamesEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}

