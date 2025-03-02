using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string MorsureInfectieuseEffectTag = "_MORSURE_INFECTIEUSE_EFFECT";
    public static Effect MorsureInfectieuse
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.MorsureInfectieuse), Effect.AbilityDecrease(Ability.Constitution, 1));
        eff.Tag = MorsureInfectieuseEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
