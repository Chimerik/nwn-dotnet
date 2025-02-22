using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string DodgeEffectTag = "_EFFECT_DODGE";
    public static Effect dodgeEffect
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.Esquive);
        eff.Tag = DodgeEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    
  }
}
