using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect disengageEffect
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.Desengagement);
        eff.Tag = DisengageffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    public const string DisengageffectTag = "_EFFECT_DISENGAGE";
  }
}
