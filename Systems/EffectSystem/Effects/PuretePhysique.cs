using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string PuretePhysiqueEffectTag = "_MONK_PURETE_PHYSIQUE_EFFECT";
    public static Effect PuretePhysique
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Immunity(ImmunityType.Poison), Effect.Immunity(ImmunityType.Disease), ImmunitePoison);
        eff.Tag = PuretePhysiqueEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
