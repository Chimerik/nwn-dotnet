using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AffaiblissementEffectTag = "_AFFAIBLISSEMENT_EFFECT";
    public static Effect Affaiblissement
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(EffectIcon.AttackDecrease), Effect.VisualEffect(VfxType.DurMindAffectingNegative));
        eff.Tag = AffaiblissementEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      } 
    }
  }
}

