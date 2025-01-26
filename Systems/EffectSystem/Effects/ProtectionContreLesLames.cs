
using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ProtectionContreLesLamesEffectTag = "_PROTECTION_CONTRE_LES_LAMES_EFFECT";
    public static Effect ProtectionContreLesLames
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.ProtectionContreLesLames);
        eff.Tag = ProtectionContreLesLamesEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}

