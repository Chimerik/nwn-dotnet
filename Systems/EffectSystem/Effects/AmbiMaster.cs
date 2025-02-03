using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AmbiMasterEffectTag = "_AMBI_MASTER_EFFECT";
    public static Effect ambiMaster
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.ACIncrease(1), Effect.Icon(CustomEffectIcon.AmbiMaster));
        eff.Tag = AmbiMasterEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
